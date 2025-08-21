import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { CreateProfileMealPlanModel } from '../../models/meal-plan-service/Requests/create-profile-meal-plan.model';
import { ProfileMealPlanWithDetailsResponse } from '../../models/meal-plan-service/Responses/profile-meal-plan-with-details.model';
import { RecommendationModel } from '../../models/meal-plan-service/Models/recommendation.model';

@Injectable({
  providedIn: 'root'
})
export class ProfilePlanService {
  private readonly baseUrl: string = 'https://localhost/meal_plan_service/ProfilePlan';

  constructor(private http: HttpClient) { }

  createProfilePlan(profilePlan: CreateProfileMealPlanModel): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}`, profilePlan);
  }

  getProfilePlanHistory(profileId: string): Observable<ProfileMealPlanWithDetailsResponse[]> {
    return this.http.get<ProfileMealPlanWithDetailsResponse[]>(`${this.baseUrl}/history`, {
      params: { profileId }
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
}
