import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { CreateMealModel } from '../../models/food-service/Requests/create-meal.model';
import { UpdateMealModel } from '../../models/food-service/Requests/update-meal.model';
import { FullMealResponse } from '../../models/food-service/Responses/full-meal.model';

@Injectable({
  providedIn: 'root'
})
export class MealService {
  private readonly baseUrl = 'https://localhost/food_service/meals';

  constructor(private http: HttpClient) { }

  getMealById(mealId: string, dayId: string): Observable<FullMealResponse> {
    return this.http.get<FullMealResponse>(this.baseUrl, { params: { mealId, dayId } });
  }

  createMeal(createMealDTO: CreateMealModel): Observable<FullMealResponse> {
    return this.http.post<FullMealResponse>(this.baseUrl, createMealDTO);
  }

  updateMeal(updateMealDTO: UpdateMealModel): Observable<void> {
    return this.http.put<void>(this.baseUrl, updateMealDTO);
  }

  deleteMeal(mealId: string, dayId: string): Observable<void> {
    return this.http.delete<void>(this.baseUrl, { params: { mealId, dayId } });
  }
}
