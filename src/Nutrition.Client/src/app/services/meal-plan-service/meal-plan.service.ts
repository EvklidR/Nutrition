import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { MealPlanModel } from '../../models/meal-plan-service/Models/meal-plan.model';
import { CreateMealPlanModel } from '../../models/meal-plan-service/Requests/create-meal-plan.model';
import { MealPlansResponseModel } from '../../models/meal-plan-service/Responces/meal-plans-response.model';
import { MealPlanType } from '../../models/meal-plan-service/Enums/meal-plan-type.enum'

@Injectable({
  providedIn: 'root'
})
export class MealPlanService {
  private readonly baseUrl: string = 'https://localhost/meal_plan_service/MealPlan';

  constructor(private http: HttpClient) { }

  getMealPlans(type: MealPlanType | null, page: number | null, size: number | null): Observable<MealPlansResponseModel> {
    let params = new HttpParams();

    if (type != null) {
      params = params.set('type', type);
    }
    if (page != null) {
      params = params.set('page', page.toString());
    }
    if (size != null) {
      params = params.set('size', size.toString());
    }
    return this.http.get<MealPlansResponseModel>(`${this.baseUrl}/meal_plans`, { params });
  }

  createMealPlan(mealPlanData: CreateMealPlanModel): Observable<MealPlanModel> {
    return this.http.post<MealPlanModel>(this.baseUrl, mealPlanData);
  }

  deleteMealPlan(mealPlanId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${mealPlanId}`);
  }

  updateMealPlan(mealPlanData: MealPlanModel): Observable<void> {
    return this.http.put<void>(this.baseUrl, mealPlanData);
  }
}
