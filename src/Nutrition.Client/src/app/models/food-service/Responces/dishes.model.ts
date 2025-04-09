import { BriefDishModel } from "./brief-dish.model";

export interface DishesResponseModel {
  dishes: BriefDishModel[] | null;
  totalCount: number;
}
