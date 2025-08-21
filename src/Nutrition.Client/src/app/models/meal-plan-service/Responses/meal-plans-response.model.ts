import { MealPlanResponseModel } from "./meal-plan-response.model";

export interface MealPlansResponseModel {
  mealPlans: MealPlanResponseModel[];
  totalCount: number;
}
