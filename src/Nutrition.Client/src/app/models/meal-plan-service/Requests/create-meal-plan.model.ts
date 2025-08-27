import { MealPlanType } from '../Enums/meal-plan-type.enum'
import { MealPlanDayModel } from '../Models/meal-plan-day.model'
import { RecommendationModel } from '../Models/recommendation.model';

export interface CreateMealPlanModel {
  name: string;
  description: string;
  type: MealPlanType;
  recommendations: RecommendationModel[];
  days: MealPlanDayModel[];
}
