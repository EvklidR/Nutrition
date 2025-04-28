import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';

import { ProfileModel } from '../../models/user-service/Models/profile.model';
import { MealPlanModel } from '../../models/meal-plan-service/Models/meal-plan.model';
import { MealPlanType } from '../../models/meal-plan-service/Enums/meal-plan-type.enum';
import { UserService } from '../../services/user-service/user.service';
import { MealPlanService } from '../../services/meal-plan-service/meal-plan.service';
import { ProfileService } from '../../services/user-service/profile.service';
import { ProfilePlanService } from '../../services/meal-plan-service/profile-plan.service';
import { ProfileMealPlanWithDetailsModel } from '../../models/meal-plan-service/Responces/profile-meal-plan-with-details.model';
import { MealPlanResponseModel } from '../../models/meal-plan-service/Responces/meal-plan-response.model';
import { CreateProfileMealPlanModel } from '../../models/meal-plan-service/Requests/create-profile-meal-plan.model';
import { CommonModule } from '@angular/common';

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

  profile: ProfileModel | null = null;
  private profileSubscription!: Subscription;

  mealPlanCategories = Object.values(MealPlanType).filter(value => typeof value === 'number') as number[];
  mealPlansByCategory: { [key: number]: { mealPlans: MealPlanResponseModel[], totalCount: number } } = {};
  currentPageByCategory: { [key: number]: number } = {};
  expandedPlanId: string | null = null;
  pageSize: number = 3;

  choosenMealPlan: ProfileMealPlanWithDetailsModel | null = null;

  isLoading: boolean = false;

  mealPlanType = MealPlanType;

  constructor(
    private router: Router,
    private userService: UserService,
    private mealPlanService: MealPlanService,
    private profileService: ProfileService,
    private profileMealPlanService: ProfilePlanService
  ) { }

  ngOnInit(): void {
    if (this.userService.isAdmin()) {
      this.userRole = 'admin'
    }

    this.profileSubscription = this.profileService.currentProfile$.subscribe((profile) => {
      this.profile = profile;

      console.log(this.profile, this.userRole)

      if (profile) {
        this.mealPlanCategories.forEach(category => {
          this.currentPageByCategory[category] = 1;
          this.loadMealPlansForCategory(category);
        });
      }
      if (profile?.thereIsMealPlan) {
        this.loadChoosenMealPlan(profile.id)
      }
      else {
        this.choosenMealPlan = null
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

  loadChoosenMealPlan(profileId: string): void {
    this.profileMealPlanService.getProfilePlanHistory(profileId).subscribe({
      next: (mealPlans) => {
        const activePlan = mealPlans.find(plan => plan.isActive);
        if (activePlan) {
          this.choosenMealPlan = activePlan
        }
        else {
          this.choosenMealPlan = null
        }
      },
      error: (err) => {
        console.log("Не удалось загрузить текущий план")
      }
    }
    )
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
        this.loadChoosenMealPlan(this.profile!.id)
        this.profile!.thereIsMealPlan = true;
      }
    )
  }

  navigateToTest(): void {
    this.router.navigate(['/test']);
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

  canselPlan() {
    this.profileMealPlanService.completeProfilePlan(this.profile!.id).subscribe(() => {
      this.loadChoosenMealPlan(this.profile!.id)
    })
  }
}
