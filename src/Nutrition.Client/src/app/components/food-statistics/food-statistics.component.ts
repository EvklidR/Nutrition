import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { sub } from 'date-fns';
import { CommonModule } from '@angular/common';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { FormsModule } from '@angular/forms';

import { ProfileService } from '../../services/user-service/profile.service';
import { DayResultService } from '../../services/food-service/day-result.service';
import { MealService } from '../../services/food-service/meal.service';
import { ShortProfileResponse } from '../../models/user-service/Responses/short-profile-response.model';
import { ShortDayResultResponse } from '../../models/food-service/Responses/short-day-result.model';
import { PeriodParameters } from '../../models/food-service/Requests/period-parameters.model';
import { EatenFoodResponse } from '../../models/food-service/Responses/eaten-food.model';

@Component({
  selector: 'app-food-statistics',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    NgxChartsModule
  ],
  templateUrl: './food-statistics.component.html',
  styleUrls: ['./food-statistics.component.css']
})
export class FoodStatisticsComponent implements OnInit {
  profile!: ShortProfileResponse | null;
  private profileSubscription!: Subscription;

  dayResults: ShortDayResultResponse[] = [];
  weeksAgo: number = 1;
  availablePeriods: number[] = [1, 2, 3, 4, 8, 12, 18, 24];

  eatenFood: EatenFoodResponse[] = []

  startDate: Date = sub(new Date(), { weeks: this.weeksAgo });
  endDate: Date = new Date();

  caloriesData: { date: string, calories: number }[] = [];
  macrosData: { date: string, proteins: number, fats: number, carbs: number }[] = [];

  chartData: any[] = [];
  colorScheme = 'cool';
  viewMode: 'calories' | 'macros' = 'calories';
  yScaleMin = 0;
  yScaleMax: number = 0;

  constructor(
    private router: Router,
    private profileService: ProfileService,
    private dayResultService: DayResultService,
    private mealService: MealService
  ) { }

  ngOnInit(): void {
    this.profileSubscription = this.profileService.currentProfile$.subscribe((profile) => {
      this.profile = profile;

      if (profile) {
        this.loadDayResults(profile.id);
      }
    });
  }

  loadDayResults(profileId: string): void {

    const periodParams: PeriodParameters = {
      startDate: this.startDate,
      endDate: this.endDate
    }

    this.dayResultService.getDayResults(profileId, periodParams, null).subscribe(
      (days) => {
        this.dayResults = days;
      }
    );
  }

  updateChartData(): void {
    if (this.viewMode === 'calories') {
      this.chartData = [
        {
          name: 'Calories',
          series: this.caloriesData.map(item => ({ name: item.date, value: item.calories }))
        }
      ];
    } else if (this.viewMode === 'macros') {
      this.chartData = [
        {
          name: 'Proteins',
          series: this.macrosData.map(item => ({ name: item.date, value: item.proteins }))
        },
        {
          name: 'Fats',
          series: this.macrosData.map(item => ({ name: item.date, value: item.fats }))
        },
        {
          name: 'Carbohydrates',
          series: this.macrosData.map(item => ({ name: item.date, value: item.carbs }))
        }
      ];
    }
  }

  navigateToBodyStatistics() {
    this.router.navigate(['/statistics']);
  }

  updatePeriod(): void {
    if (this.profile) {
      this.startDate = sub(this.endDate, { weeks: Number(this.weeksAgo) });
      this.loadDayResults(this.profile.id);
    }
  }

  switchViewMode(mode: 'calories' | 'macros'): void {
    this.viewMode = mode;
    this.updateChartData();
  }

  getTopFoods(): void {
    if (this.profile) {
      this.dayResultService.getEatenFood(this.profile!.id, { startDate: this.startDate, endDate: this.endDate }).subscribe(
        (food) => {
          this.eatenFood = food
        }
      )
    }
  }
}
