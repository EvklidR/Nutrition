import { ProductOfDishModel } from "./product-of-dish.model";

export interface CreateDishModel {
  userId: string;
  name: string;
  description: string | null;
  amountOfPortions: number;
  image: File | null;
  ingredients: ProductOfDishModel[];
}
