import { Component, OnInit } from '@angular/core';
import { sub } from 'date-fns';
import { CommonModule } from '@angular/common';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { FormsModule } from '@angular/forms';

import { ProfileService } from '../../../services/user-service/profile.service';
import { DayResultService } from '../../../services/food-service/day-result.service';
import { ShortProfileResponse } from '../../../models/user-service/Responses/short-profile-response.model';
import { ShortDayResultResponse } from '../../../models/food-service/Responses/short-day-result.model';
import { EatenFoodResponse } from '../../../models/food-service/Responses/eaten-food.model';
import addDays from 'date-fns/esm/addDays/index.js';
import format from 'date-fns/format/index';
import { PeriodParameters } from '../../../models/request-parameters/period-parameters.model';
import { DateRangePickerComponent } from '../../date-range-picker/date-range-picker.component';

@Component({
  selector: 'app-food-statistics',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    NgxChartsModule,
    DateRangePickerComponent
  ],
  templateUrl: './food-statistics.component.html',
  styleUrls: ['./food-statistics.component.css']
})
export class FoodStatisticsComponent implements OnInit {
  profile!: ShortProfileResponse | null;

  dayResults: ShortDayResultResponse[] = [];

  eatenFood: EatenFoodResponse[] = []

  periodParams: PeriodParameters = {
    startDate: format(sub(new Date(), { days: 6 }), "yyyy-MM-dd"),
    endDate: format(new Date(), "yyyy-MM-dd")
  }

  caloriesData: { date: string, calories: number }[] = [];
  macrosData: { date: string, proteins: number, fats: number, carbs: number }[] = [];

  chartData: any[] = [];
  colorScheme = 'cool';
  viewMode: 'calories' | 'macros' = 'calories';
  yScaleMin = 0;
  yScaleMax: number = 0;

  sortKey: 'weight' | 'calories' | 'proteins' | 'fats' | 'carbohydrates' = 'calories';
  sortDirection: 'asc' | 'desc' = 'desc';

  constructor(
    private profileService: ProfileService,
    private dayResultService: DayResultService,
  ) { }

  ngOnInit(): void {
    this.profileService.currentProfile$.subscribe((profile) => {
      this.profile = profile;

      if (profile) {
        this.loadDayResults();
        this.getTopFoods()
      }
    });
  }

  onUpdatePeriod() {
    this.loadDayResults();
    this.getTopFoods()
  }

  loadDayResults(): void {
    this.dayResultService.getDayResults(this.profile!.id, this.periodParams, null).subscribe(
      (response) => {
        this.dayResults = response.dayResults;

        for (let currentDate = addDays(new Date(this.periodParams.startDate), 1); currentDate <= new Date(this.periodParams.endDate); currentDate = addDays(currentDate, 1)) {

          let day: ShortDayResultResponse | undefined = this.dayResults.find(
            d => this.toDateString(d.date) === this.toDateString(currentDate)
          );

          if (!day) {
            this.dayResults.push({
              date: format(currentDate, "yyyy-MM-dd"),
              calories: 0,
              carbohydrates: 0,
              proteins: 0,
              fats: 0,
              weight: 0,
              id: ""
            })
          }
        }

        this.dayResults.sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());

        this.updateChartData()
      }
    );
  }

  updateChartData(): void {
    if (this.viewMode === 'calories') {
      this.chartData = [
        {
          name: 'Calories',
          series: this.dayResults.map(item => ({ name: item.date, value: item.calories }))
        }
      ];
    } else if (this.viewMode === 'macros') {
      this.chartData = [
        {
          name: 'Proteins',
          series: this.dayResults.map(item => ({ name: item.date, value: item.proteins }))
        },
        {
          name: 'Fats',
          series: this.dayResults.map(item => ({ name: item.date, value: item.fats }))
        },
        {
          name: 'Carbohydrates',
          series: this.dayResults.map(item => ({ name: item.date, value: item.carbohydrates }))
        }
      ];
    }
  }

  switchViewMode(mode: 'calories' | 'macros'): void {
    this.viewMode = mode;
    this.updateChartData();
  }

  getTopFoods(): void {
    if (this.profile) {
      this.dayResultService.getEatenFood(this.profile!.id, this.periodParams).subscribe(
        (food) => {
          this.eatenFood = food
        }
      )
    }
  }

  toDateString(date: Date | string): string {
    if (typeof date === 'string') return date;
    return format(date, "yyyy-MM-dd")
  }

  get sortedTopFood() {
    return [...this.eatenFood]
      .sort((a, b) => {
        let valA: number, valB: number;

        switch (this.sortKey) {
          case 'weight': valA = a.totalWeight; valB = b.totalWeight; break;
          case 'calories': valA = a.totalCalories; valB = b.totalCalories; break;
          case 'proteins': valA = a.totalProteins; valB = b.totalProteins; break;
          case 'fats': valA = a.totalFats; valB = b.totalFats; break;
          case 'carbohydrates': valA = a.totalCarbohydrates; valB = b.totalCarbohydrates; break;
        }

        return this.sortDirection === 'asc' ? valA - valB : valB - valA;
      })
      .slice(0, 15);
  }

  setSort(key: typeof this.sortKey) {
    if (this.sortKey === key) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortKey = key;
      this.sortDirection = 'desc';
    }
  }
}
