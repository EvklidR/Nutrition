import { ProductOfRecipeModel } from "./product-of-recipe.model";

export interface CreateRecipeModel {
  name: string;
  description: string | null;
  amountOfPortions: number;
  image: File | null;
  ingredients: ProductOfRecipeModel[];
}
