export interface ProfileMealPlanWithDetailsResponse {
  id: string,
  mealPlanId: string,
  profileId: string,
  isActive: boolean,
  startDate: Date,
  endDate: Date,
  mealPlanName: string,
  mealPlanDescription: string
}
