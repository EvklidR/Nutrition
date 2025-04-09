import { MealPlanType } from "../Enums/meal-plan-type.enum";

export interface MealPlanResponseModel {
  id: string;
  name: string;
  description: string;
  type: MealPlanType
}
