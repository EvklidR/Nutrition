import { MealFoodModel } from "./meal-food.model";

export interface FullMealModel {
  id: string;
  name: string;
  totalCalories: number;
  totalProteins: number;
  totalFats: number;
  totalCarbohydrates: number;
  products: MealFoodModel[];
  dishes: MealFoodModel[];
}
