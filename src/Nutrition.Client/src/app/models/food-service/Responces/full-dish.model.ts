import { DishProductModel } from "./dish-product.model";

export interface FullDishModel {
  id: string;
  name: string;
  description: string | null;
  imageUrl: string | null;
  calories: number;
  proteins: number;
  fats: number;
  carbohydrates: number;
  weight: number;
  ingredients: DishProductModel[];
}
