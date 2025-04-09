export interface ProfileMealPlanModel {
  id: string,
  mealPlanId: string,
  profileId: string,
  isActive: boolean,
  startDate: Date,
  endDate: Date
}
