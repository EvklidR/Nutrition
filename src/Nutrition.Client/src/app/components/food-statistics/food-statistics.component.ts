import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { sub, format, addDays } from 'date-fns';
import { CommonModule } from '@angular/common';

import { ProfileModel } from '../../models/user-service/Models/profile.model';
import { DayResultModel } from '../../models/food-service/Responces/day-result.model';
import { ProfileService } from '../../services/user-service/profile.service';
import { DayResultService } from '../../services/food-service/day-result.service';
import { MealService } from '../../services/food-service/meal.service';
import { FullMealModel } from '../../models/food-service/Responces/full-meal.model';
import { FormsModule } from '@angular/forms';
import { NgxChartsModule } from '@swimlane/ngx-charts';

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
  profile!: ProfileModel | null;
  private profileSubscription!: Subscription;

  dayResults: DayResultModel[] = [];
  weeksAgo: number = 1;
  availablePeriods: number[] = [1, 2, 3, 4, 8, 12, 18, 24];

  start: Date = sub(new Date(), { weeks: this.weeksAgo });
  end: Date = new Date();

  caloriesData: { date: string, calories: number }[] = [];
  macrosData: { date: string, proteins: number, fats: number, carbs: number }[] = [];

  chartData: any[] = [];
  colorScheme = 'cool';
  viewMode: 'calories' | 'macros' = 'calories';
  yScaleMin = 0;
  yScaleMax: number = 0;

  mealCache: Map<string, FullMealModel> = new Map();

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

  get startDate(): string {
    return format(this.start, 'yyyy-MM-dd');
  }

  get endDate(): string {
    return format(this.end, 'yyyy-MM-dd');
  }

  loadDayResults(profileId: string): void {

    this.dayResultService.getDayResultsByPeriod(profileId, this.startDate, this.endDate).subscribe(
      (days) => {
        this.dayResults = days;
        this.generateData();
      }
    );
  }

  generateData(): void {
    this.caloriesData = [];
    this.macrosData = [];

    this.dayResults = this.dayResults.map(d => ({
      ...d,
      date: new Date(d.date)
    }));

    for (let currentDate = addDays(this.start, 1); currentDate <= this.end; currentDate = addDays(currentDate, 1)) {
      const formattedDate = format(currentDate, 'yyyy-MM-dd');

      let dayResult: DayResultModel | undefined = this.dayResults.find(
        d => format(d.date, 'yyyy-MM-dd') === formattedDate
      );

      if (dayResult) {
        const totalCalories = dayResult.meals.reduce((sum, meal) => sum + meal.totalCalories, 0);

        const totalProteins = dayResult.meals.reduce((sum, meal) => sum + meal.totalProteins, 0);

        const totalFats = dayResult.meals.reduce((sum, meal) => sum + meal.totalFats, 0);

        const totalCarbs = dayResult.meals.reduce((sum, meal) => sum + meal.totalCarbohydrates, 0);

        this.caloriesData.push({ date: formattedDate, calories: totalCalories });
        this.macrosData.push({ date: formattedDate, proteins: totalProteins, fats: totalFats, carbs: totalCarbs });
      } else {
        this.caloriesData.push({ date: formattedDate, calories: 0 });
        this.macrosData.push({ date: formattedDate, proteins: 0, fats: 0, carbs: 0 });
      }
    }

    this.updateChartData();
    this.generateTopFoods();
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
      this.start = sub(this.end, { weeks: Number(this.weeksAgo) });
      this.loadDayResults(this.profile.id);
    }
  }

  switchViewMode(mode: 'calories' | 'macros'): void {
    this.viewMode = mode;
    this.updateChartData();
  }

  topFoods: { name: string, weight: number, calories: number, proteins: number, fats: number, carbs: number }[] = [];

  generateTopFoods(): void {
    const foodMap = new Map<string, { weight: number, calories: number, proteins: number, fats: number, carbs: number }>();

    for (const dayResult of this.dayResults) {
      for (const meal of dayResult.meals) {
        const fullMeal = this.mealCache.get(meal.id);

        if (fullMeal) {
          this.addFoodStats(foodMap, fullMeal);
        } else {
          this.mealService.getMealById(meal.id, dayResult.id).subscribe((mealDetails) => {
            this.mealCache.set(meal.id, mealDetails);
            this.addFoodStats(foodMap, mealDetails);

            // Обновляем топ только после загрузки всех
            this.topFoods = Array.from(foodMap.entries())
              .map(([name, stats]) => ({ name, ...stats }))
              .sort((a, b) => b.weight - a.weight)
              .slice(0, 15);
          });
        }
      }
    }

    // Обновим сразу по кэшу (на случай, если всё уже загружено)
    this.topFoods = Array.from(foodMap.entries())
      .map(([name, stats]) => ({ name, ...stats }))
      .sort((a, b) => b.weight - a.weight)
      .slice(0, 15);
  }

  addFoodStats(
    foodMap: Map<string, { weight: number, calories: number, proteins: number, fats: number, carbs: number }>,
    fullMeal: FullMealModel
  ) {
    for (const product of fullMeal.products) {
      const name = product.name;
      this.updateFoodMap(foodMap, name, product.weight, product.totalProductCalories, product.totalProductProteins, product.totalProductFats, product.totalProductCarbohydrates);
    }

    for (const dish of fullMeal.dishes) {
      const name = dish.name;
      this.updateFoodMap(foodMap, name, dish.weight, dish.totalProductCalories, dish.totalProductProteins, dish.totalProductFats, dish.totalProductCarbohydrates);
    }
  }

  updateFoodMap(
    foodMap: Map<string, { weight: number, calories: number, proteins: number, fats: number, carbs: number }>,
    name: string,
    weight: number,
    calories: number,
    proteins: number,
    fats: number,
    carbs: number
  ) {
    if (foodMap.has(name)) {
      const existing = foodMap.get(name)!;
      foodMap.set(name, {
        weight: existing.weight + weight,
        calories: existing.calories + calories,
        proteins: existing.proteins + proteins,
        fats: existing.fats + fats,
        carbs: existing.carbs + carbs,
      });
    } else {
      foodMap.set(name, { weight, calories, proteins, fats, carbs });
    }
  }

}
