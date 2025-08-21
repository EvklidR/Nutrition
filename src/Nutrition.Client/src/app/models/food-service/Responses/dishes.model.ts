import { DishResponse } from "./dish.model";

export interface DishesResponse {
  dishes: DishResponse[];
  totalCount: number;
}
