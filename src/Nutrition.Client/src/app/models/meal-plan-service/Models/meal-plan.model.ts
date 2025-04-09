import { MealPlanType } from '../Enums/meal-plan-type.enum'
import { MealPlanDayModel } from '../Models/meal-plan-day.model'

export interface MealPlanModel {
  id: string;
  name: string;
  description: string;
  type: MealPlanType;
  days: MealPlanDayModel[];
}
