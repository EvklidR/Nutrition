import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { NgIf, NgFor, CommonModule } from "@angular/common"
import { faTrash } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

import { UserService } from '../../services/user-service/user.service';
import { MatDialog } from '@angular/material/dialog';
import { MealDetailsModalComponent } from '../modals/meal-details-modal/meal-details-modal.component';
import { DayResultService } from '../../services/food-service/day-result.service';
import { ProfileService } from '../../services/user-service/profile.service';
import { MealService } from '../../services/food-service/meal.service';
import { DayResultModel } from '../../models/food-service/Responces/day-result.model';
import { ProfileModel } from '../../models/user-service/Models/profile.model';
import { DailyNeedsResponse } from '../../models/user-service/Responces/daily-needs.model';
import { UpdateProfileModel } from '../../models/user-service/Requests/update-profile.model';
import { UpdateDayResultModel } from '../../models/food-service/Requests/update-day-result.model';
import { CreateMealComponent } from '../modals/create-meal-modal/create-meal.component';
import { BriefMealModel } from '../../models/food-service/Responces/brief-meal.model';


@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    NgIf,
    NgFor,
    CommonModule,
    FontAwesomeModule
  ],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  faTrash = faTrash;

  dayResult!: DayResultModel | null;
  profile!: ProfileModel | null;
  dayResultId!: string | null;

  dailyNeeads!: DailyNeedsResponse;

  totalNutrients = {
    calories: 0,
    proteins: 0,
    fats: 0,
    carbohydrates: 0
  };

  showAddMealModal: boolean = false

  private profileSubscription!: Subscription;

  constructor(
    private userService: UserService,
    private router: Router,
    private dayResultService: DayResultService,
    private profileService: ProfileService,
    private mealService: MealService,
    private dialog: MatDialog,
  ) { }

  ngOnInit(): void {
    if (!this.userService.isAuthenticated()) {
      this.router.navigate(['/login']);
      return;
    }

    this.profileSubscription = this.profileService.currentProfile$.subscribe((profile) => {
      this.profile = profile;
      console.log("инициализация хоум, текущий профиль:", profile)
      if (profile) {
        this.profileService.calculateDailyNeeds(profile.id).subscribe((dailyNeeads) => {
          console.log(dailyNeeads)
          this.dailyNeeads = dailyNeeads;
          this.getOrCreateDayResult();
        });
      }
    });
  }

  onMealAdded(): void {
    this.getOrCreateDayResult()
  }

  getOrCreateDayResult(): void {
    this.dayResultService.getOrCreateDayResult(this.profile!.id).subscribe({
      next: (result) => {
        this.dayResult = result;
        this.dayResultId = result.id;

        var calories = 0;
        var proteins = 0;
        var fats = 0;
        var carbohydrates = 0;

        if (this.dayResult.meals) {
          this.dayResult.meals.forEach((meal) => {
            calories += meal.totalCalories;
            proteins += meal.totalProteins;
            fats += meal.totalFats;
            carbohydrates += meal.totalCarbohydrates;
          });
        }

        this.totalNutrients.calories = calories;
        this.totalNutrients.proteins = proteins;
        this.totalNutrients.fats = fats;
        this.totalNutrients.carbohydrates = carbohydrates;
      },

      error: (err) => {
        console.error('Ошибка при получении или создании дневного результата:', err);
        this.dayResult = null;
      },
    });
  }

  getAge(birthday: string): number {
    const birthDate = new Date(birthday);
    const today = new Date();
    let age = today.getFullYear() - birthDate.getFullYear();
    if (
      today.getMonth() < birthDate.getMonth() ||
      (today.getMonth() === birthDate.getMonth() && today.getDate() < birthDate.getDate())
    ) {
      age--;
    }
    return age;
  }

  getDashArray(currentCalories: number, totalCalories: number): string {
    const radius = 70;
    const circumference = 2 * Math.PI * radius;

    const progress = totalCalories === 0 ? 0 : (currentCalories * 100 / totalCalories);

    const dashArray = (progress / 100) * circumference;

    return `${dashArray}, ${circumference}`;
  }

  getPercentage(current: number, total: number): number {
    return total !== 0 ? (current / total) * 100 : 0;
  }

  openMealDetails(meal: BriefMealModel): void {
    const dialogRef = this.dialog.open(MealDetailsModalComponent, {
      width: '900px',
      maxWidth: '900px',
      minWidth: '830px',
      height: '500px',
      data: { mealId: meal.id, dayId: this.dayResultId }
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('Диалог закрыт');
    });
  }

  openCreationMealModal(): void {
    const dialogRef = this.dialog.open(CreateMealComponent, {
      width: '900px',
      maxWidth: '900px',
      minWidth: '830px',
      height: '500px',
      data: { dayResultId: this.dayResultId }
    });

    dialogRef.afterClosed().subscribe(result => {
      this.getOrCreateDayResult()
      console.log('Диалог закрыт');
    });
  }

  removeMeal(mealId: string, event: Event): void {
    event.stopPropagation();
    if (this.dayResult) {
      this.mealService.deleteMeal(mealId, this.dayResult.id).subscribe({
        next: () => {
          if (this.dayResult) {
            this.dayResult.meals = this.dayResult.meals.filter(meal => meal.id !== mealId);

            this.getOrCreateDayResult();
            console.log("Deleted successfully");
          }
        },
        error: (error) => {
          console.error("Error while deleting", error);
        }
      });
    }
  }

  adjustWaterGoal(action: 'increase' | 'decrease'): void {
    if (!this.profile) {
      console.error('Profile is not defined');
      return;
    }

    if (action === 'increase') {
      this.profile.desiredGlassesOfWater++;
    } else if (action === 'decrease' && this.profile.desiredGlassesOfWater > 0) {
      this.profile.desiredGlassesOfWater--;
    }

    let profile: UpdateProfileModel =
    {
      id: this.profile.id,
      name: this.profile.name,
      weight: this.profile.weight,
      height: this.profile.height,
      activityLevel: this.profile.activityLevel,
      desiredGlassesOfWater: this.profile.desiredGlassesOfWater
    }

    this.profileService.updateProfile(profile).subscribe({
      next: () => {
        console.log('Water goal updated successfully:', profile);
      },
      error: (error) => {
        console.error('Error updating water goal:', error);
      }
    });
  }

  drinkWater(): void {

    if (!this.dayResult) {
      console.error('No day result available for drinking water.');
      return;
    }

    const updateDayResultDTO: UpdateDayResultModel = {
      id: this.dayResult.id,
      glassesOfWater: this.dayResult.glassesOfWater + 1,
      weight: this.dayResult.weight,
    };

    this.dayResultService.updateDayResult(updateDayResultDTO).subscribe({
      next: () => {
        console.log('Glass of water added.');
        this.dayResult!.glassesOfWater += 1;
      },
      error: (err) => {
        console.error('Error adding glass of water:', err);
      },
    });
  }
}
