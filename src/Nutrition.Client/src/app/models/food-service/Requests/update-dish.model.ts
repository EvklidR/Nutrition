import { ProductOfDishModel } from "./product-of-dish.model";

export interface UpdateDishModel {
  id: string;
  name: string;
  description: string | null;
  amountOfPortions: number;
  image: File | null;
  deleteImageIfNull: boolean;
  ingredients: ProductOfDishModel[];
}
