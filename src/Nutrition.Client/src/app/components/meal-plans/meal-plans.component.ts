import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { MealPlanType } from '../../models/meal-plan-service/Enums/meal-plan-type.enum';
import { UserService } from '../../services/user-service/user.service';
import { MealPlanService } from '../../services/meal-plan-service/meal-plan.service';
import { ProfileService } from '../../services/user-service/profile.service';
import { ProfilePlanService } from '../../services/meal-plan-service/profile-plan.service';
import { MealPlanResponseModel } from '../../models/meal-plan-service/Responses/meal-plan-response.model';
import { CreateProfileMealPlanModel } from '../../models/meal-plan-service/Requests/create-profile-meal-plan.model';
import { CommonModule } from '@angular/common';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ShortProfileResponse } from '../../models/user-service/Responses/short-profile-response.model';
import { ProfileMealPlanWithDetailsResponse } from '../../models/meal-plan-service/Responses/profile-meal-plan-with-details.model';

@Component({
  selector: 'app-meal-plans',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './meal-plans.component.html',
  styleUrls: ['./meal-plans.component.css'],
})
export class MealPlansComponent implements OnInit {
  userRole: string | null = null;

  profile: ShortProfileResponse | null = null;

  mealPlanCategories = Object.values(MealPlanType).filter(value => typeof value === 'number') as number[];
  mealPlansByCategory: { [key: number]: { mealPlans: MealPlanResponseModel[], totalCount: number } } = {};
  currentPageByCategory: { [key: number]: number } = {};
  expandedPlanId: string | null = null;
  pageSize: number = 3;

  choosenMealPlan: ProfileMealPlanWithDetailsResponse | null = null;

  isLoading: boolean = false;

  mealPlanType = MealPlanType;

  constructor(
    private router: Router,
    private userService: UserService,
    private mealPlanService: MealPlanService,
    private profileService: ProfileService,
    private profileMealPlanService: ProfilePlanService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    if (this.userService.isAdmin()) {
      this.userRole = 'admin'
    }

    this.mealPlanCategories.forEach(category => {
      this.currentPageByCategory[category] = 1;
      this.loadMealPlansForCategory(category);
    });

    this.profileService.currentProfile$.subscribe((profile) => {
      this.profile = profile;

      console.log(this.profile, this.userRole)

      if (profile) {
        this.loadChoosenMealPlan()
      }
    })
  }

  loadMealPlansForCategory(category: MealPlanType): void {
    const pageNumber = this.currentPageByCategory[category];

    this.isLoading = true;
    this.mealPlanService
      .getMealPlans(category, pageNumber, this.pageSize)
      .subscribe({
        next: (data) => {

          if (!this.mealPlansByCategory[category]) {
            this.mealPlansByCategory[category] = { mealPlans: [], totalCount: 0 };
          }

          this.mealPlansByCategory[category].mealPlans = data.mealPlans;
          this.mealPlansByCategory[category].totalCount = data.totalCount;

          this.isLoading = false;
        },
        error: (error) => {
          console.error(`Ошибка загрузки планов питания для категории ${category}:`, error);
          this.isLoading = false;
        }
      });
  }

  nextPage(category: MealPlanType): void {
    this.currentPageByCategory[category]++;
    this.loadMealPlansForCategory(category);
  }

  prevPage(category: MealPlanType): void {
    if (this.currentPageByCategory[category] > 1) {
      this.currentPageByCategory[category]--;
      this.loadMealPlansForCategory(category);
    }
  }

  toggleDescription(planId: string): void {
    this.expandedPlanId = this.expandedPlanId === planId ? null : planId;
  }

  getCategoryName(category: MealPlanType): string {
    switch (category) {
      case MealPlanType.WeightLoss:
        return 'Потеря веса';
      case MealPlanType.WeightGain:
        return 'Набор веса';
      case MealPlanType.MuscleGain:
        return 'Наращивание мышц';
      case MealPlanType.Maintenance:
        return 'Поддержание формы';
      default:
        return 'Неизвестная категория';
    }
  }

  selectPlan(plan: MealPlanResponseModel, event: Event): void {
    event.stopPropagation();

    const createProfilePlan: CreateProfileMealPlanModel = {
      mealPlanId: plan.id,
      profileId: this.profile!.id
    }

    this.profileMealPlanService.createProfilePlan(createProfilePlan).subscribe(
      () => {
        console.log("Новая подписка успешна")
        this.loadChoosenMealPlan()
        this.sendPopUpNotification("План питания успешно выбран!")
      }
    )
  }

  navigateToMyPlans(): void {
    this.router.navigate(['/my-meal-plans']);
  }

  isLastPage(category: MealPlanType): boolean {
    const currentPage = this.currentPageByCategory[category];
    const totalCount = this.mealPlansByCategory[category]?.totalCount || 0;

    const totalPages = Math.ceil(totalCount / this.pageSize);
    return currentPage >= totalPages;
  }

  navigateToAddingPlan() {
    this.router.navigate(["/create-meal-plan"])
  }

  navigateToEditingPlan(id: string) {
    this.router.navigate(['/meal-plan/edit', id]);
  }

  canselPlan() {
    this.profileMealPlanService.completeProfilePlan(this.profile!.id).subscribe(() => {
      this.loadChoosenMealPlan()
      this.sendPopUpNotification("План питания успешно отменен!")
    })
  }

  sendPopUpNotification(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000
    });
    return
  }

  loadChoosenMealPlan() {
    this.profileMealPlanService.getActiveMealPlan(this.profile!.id).subscribe(
      (profileMealPlan) => {
        this.choosenMealPlan = profileMealPlan
      }
    )
  }
}
