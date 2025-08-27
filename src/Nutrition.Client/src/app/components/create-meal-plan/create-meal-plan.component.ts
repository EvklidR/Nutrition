import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CreateMealPlanModel } from '../../models/meal-plan-service/Requests/create-meal-plan.model';
import { CalculationType } from '../../models/meal-plan-service/Enums/calculation-type.enum';
import { MealPlanType } from '../../models/meal-plan-service/Enums/meal-plan-type.enum';
import { MealPlanService } from '../../services/meal-plan-service/meal-plan.service';
import { NutrientType } from '../../models/meal-plan-service/Enums/nutrient-type.enum';
import { MealPlanDayModel } from '../../models/meal-plan-service/Models/meal-plan-day.model';
import { FormsModule } from '@angular/forms';
import { MealPlanModel } from '../../models/meal-plan-service/Models/meal-plan.model';

@Component({
  selector: 'app-create-meal-plan',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './create-meal-plan.component.html',
  styleUrls: ['./create-meal-plan.component.css']
})
export class CreateMealPlanComponent implements OnInit {
  createMealPlan!: CreateMealPlanModel;
  isEditMode = false;
  mealPlanId: string | null = null;

  nutrientTypes = ['Белки', 'Жиры', 'Углеводы'];
  calculationTypes = CalculationType;
  mealPlanType = MealPlanType;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private mealPlanService: MealPlanService
  ) {}

  ngOnInit(): void {
    this.mealPlanId = this.route.snapshot.paramMap.get('id');

    if (this.mealPlanId) {
      this.isEditMode = true;
      this.loadMealPlan(this.mealPlanId);
    } else {
      this.isEditMode = false;
      this.createMealPlan = {
        name: '',
        description: '',
        type: MealPlanType.Maintenance,
        recommendations: [],
        days: []
      };
      this.addDay();
    }
  }

  loadMealPlan(id: string) {
    this.mealPlanService.getMealPlanById(id).subscribe((plan: MealPlanModel) => {
      this.createMealPlan = {
        name: plan.name,
        description: plan.description,
        type: plan.type,
        recommendations: plan.recommendations,
        days: plan.days
      };
    });
  }

  addDay() {
    const newDay: MealPlanDayModel = {
      dayNumber: this.createMealPlan.days.length + 1,
      caloriePercentage: 1,
      recommendations: [],
      nutrientsOfDay: [
        { nutrientType: NutrientType.Protein, calculationType: CalculationType.Percent, value: 0.2 },
        { nutrientType: NutrientType.Fat, calculationType: CalculationType.Percent, value: 0.3 },
        { nutrientType: NutrientType.Carbohydrate, calculationType: CalculationType.Percent, value: 0.5 }
      ]
    };
    this.createMealPlan.days.push(newDay);
  }

  deleteDay(dayIndex: number) {
    if (this.createMealPlan.days.length > 1) {
      this.createMealPlan.days.splice(dayIndex, 1);
      this.createMealPlan.days.forEach((day, index) => {
        day.dayNumber = index + 1;
      });
    } else {
      alert('Невозможно удалить последний день!');
    }
  }

  onSubmit() {
    this.createMealPlan.type = Number(this.createMealPlan.type);

    if (this.isEditMode && this.mealPlanId) {
      const updatedPlan: MealPlanModel = {
        id: this.mealPlanId,
        ...this.createMealPlan
      };
      this.mealPlanService.updateMealPlan(updatedPlan).subscribe(() => {
        console.log("обновлено");
        this.router.navigate(["/meal-plans"]);
      });
    } else {
      // создание
      this.mealPlanService.createMealPlan(this.createMealPlan).subscribe((plan) => {
        console.log("создан ", plan);
        this.router.navigate(["/meal-plans"]);
      });
    }
  }

  addPlanRecommendation() {
    this.createMealPlan.recommendations.push({ text: '' });
  }

  deletePlanRecommendation(index: number) {
    this.createMealPlan.recommendations.splice(index, 1);
  }

  addDayRecommendation(dayIndex: number) {
    this.createMealPlan.days[dayIndex].recommendations.push({ text: '' });
  }

  deleteDayRecommendation(dayIndex: number, recIndex: number) {
    this.createMealPlan.days[dayIndex].recommendations.splice(recIndex, 1);
  }

  getCalculationDescription(calculationType: CalculationType): string {
    switch (calculationType) {
      case CalculationType.Fixed:
        return 'Фиксированное значение';
      case CalculationType.Percent:
        return 'Процент от общей калорийности';
      case CalculationType.PerKg:
        return 'Значение на килограмм массы';
      case CalculationType.ByDefault:
        return 'Оставшееся количество после других нутриентов';
      default:
        return '';
    }
  }
}
