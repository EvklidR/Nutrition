import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { FullMealModel } from '../../models/food-service/Responces/full-meal.model';
import { CreateMealModel } from '../../models/food-service/Requests/create-meal.model';
import { UpdateMealModel } from '../../models/food-service/Requests/update-meal.model';

@Injectable({
  providedIn: 'root'
})
export class MealService {
  private readonly baseUrl = 'https://localhost/food_service/meals';

  constructor(private http: HttpClient) { }

  getMealById(mealId: string, dayId: string): Observable<FullMealModel> {
    return this.http.get<FullMealModel>(this.baseUrl, { params: { mealId, dayId } });
  }

  createMeal(createMealDTO: CreateMealModel): Observable<FullMealModel> {
    return this.http.post<FullMealModel>(this.baseUrl, createMealDTO);
  }

  updateMeal(updateMealDTO: UpdateMealModel): Observable<void> {
    return this.http.put<void>(this.baseUrl, updateMealDTO);
  }

  deleteMeal(mealId: string, dayId: string): Observable<void> {
    return this.http.delete<void>(this.baseUrl, { params: { mealId, dayId } });
  }
}
