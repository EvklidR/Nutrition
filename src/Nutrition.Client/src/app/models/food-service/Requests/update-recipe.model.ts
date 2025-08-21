import { ProductOfRecipeModel } from "./product-of-recipe.model";

export interface UpdateRecipeModel {
  id: string;
  name: string;
  description: string | null;
  amountOfPortions: number;
  image: File | null;
  deleteImageIfNull: boolean;
  ingredients: ProductOfRecipeModel[];
}
