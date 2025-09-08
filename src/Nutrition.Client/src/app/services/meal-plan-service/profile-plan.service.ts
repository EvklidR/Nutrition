import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { CreateProfileMealPlanModel } from '../../models/meal-plan-service/Requests/create-profile-meal-plan.model';
import { ProfileMealPlanWithDetailsResponse } from '../../models/meal-plan-service/Responses/profile-meal-plan-with-details.model';
import { RecommendationModel } from '../../models/meal-plan-service/Models/recommendation.model';
import { ProfileMealPlansResponse } from '../../models/meal-plan-service/Responses/profile-meal-plans.model';
import { PaginationParameters } from '../../models/request-parameters/pagination-parameters.model';
import { PeriodParameters } from '../../models/request-parameters/period-parameters.model';
import format from 'date-fns/esm/format/index.js';

@Injectable({
  providedIn: 'root'
})
export class ProfilePlanService {
  private readonly baseUrl: string = 'https://localhost/meal_plan_service/ProfilePlan';

  constructor(private http: HttpClient) { }

  createProfilePlan(profilePlan: CreateProfileMealPlanModel): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}`, profilePlan);
  }

  getProfilePlanHistory(
    profileId: string,
    paginationParameters: PaginationParameters | null,
    periodParameters: PeriodParameters | null): Observable<ProfileMealPlansResponse> {

    let params = this.buildParams({ ...periodParameters, ...paginationParameters });
    params = params.append("profileId", profileId); // ← правильно

    return this.http.get<ProfileMealPlansResponse>(`${this.baseUrl}/history`, {
      params
    });
  }

  completeProfilePlan(profileId: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/complete`, null, {
      params: { profileId }
    });
  }

  getRecommendations(profileId: string): Observable<RecommendationModel[]> {
    return this.http.get<RecommendationModel[]>(`${this.baseUrl}/recommendations`, {
      params: { profileId }
    });
  }

  getActiveMealPlan(profileId: string): Observable<ProfileMealPlanWithDetailsResponse | null> {
    return this.http.get<ProfileMealPlanWithDetailsResponse | null>(`${this.baseUrl}/active-plan/${profileId}`);
  }

  private buildParams(obj: any): HttpParams {
    let params = new HttpParams();

    for (const key in obj) {
      if (obj[key] !== null && obj[key] !== undefined) {
        let value = obj[key];
        params = params.set(key, String(value));
      }
    }

    return params;
  }
}
