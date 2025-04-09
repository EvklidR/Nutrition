import { RecommendationModel } from './recommendation.model'
import { NutrientOfDayModel } from './nutrient-of-day.model'

export interface MealPlanDayModel {
  dayNumber: number;
  caloriePercentage: number | null;
  recommendations: RecommendationModel[];
  nutrientsOfDay: NutrientOfDayModel[];
}
