import { ProfileMealPlanWithDetailsResponse } from "./profile-meal-plan-with-details.model";

export interface ProfileMealPlansResponse {
  profileMealPlans: ProfileMealPlanWithDetailsResponse[]
  totalCount: number
}
