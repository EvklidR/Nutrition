import { MealDishResponse } from "./meal-dish.model";
import { MealProductResponse } from "./meal-product.model";

export interface FullMealResponse {
  id: string;
  name: string | null;
  totalCalories: number;
  totalProteins: number;
  totalFats: number;
  totalCarbohydrates: number;
  products: MealProductResponse[];
  dishes: MealDishResponse[];
}
