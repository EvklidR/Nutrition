import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateProfileMealPlanModel } from '../../models/meal-plan-service/Requests/create-profile-meal-plan.model';
import { ProfileMealPlanModel } from '../../models/meal-plan-service/Responces/profile-meal-plan.model';
import { RecommendationModel } from '../../models/meal-plan-service/Responces/recommendation.model';

@Injectable({
  providedIn: 'root'
})
export class ProfilePlanService {
  private readonly baseUrl: string = 'https://localhost/meal_plan_service/profileplan';

  constructor(private http: HttpClient) { }

  createProfilePlan(profilePlan: CreateProfileMealPlanModel): Observable<ProfileMealPlanModel> {
    return this.http.post<ProfileMealPlanModel>(`${this.baseUrl}`, profilePlan);
  }

  getProfilePlanHistory(profileId: string): Observable<ProfileMealPlanModel[]> {
    return this.http.get<ProfileMealPlanModel[]>(`${this.baseUrl}/history`, {
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
}
