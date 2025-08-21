import { RecipeResponse } from "./recipe.model";

export interface CalculatedRecipeResponse extends RecipeResponse {
  calories: number;
  proteins: number;
  fats: number;
  carbohydrates: number;
}
