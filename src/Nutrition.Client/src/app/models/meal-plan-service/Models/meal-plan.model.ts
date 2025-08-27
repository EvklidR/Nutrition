import { MealPlanType } from '../Enums/meal-plan-type.enum'
import { MealPlanDayModel } from '../Models/meal-plan-day.model'
import { RecommendationModel } from './recommendation.model';

export interface MealPlanModel {
  id: string;
  name: string;
  description: string;
  type: MealPlanType;
  recommendations: RecommendationModel[];
  days: MealPlanDayModel[];
}
