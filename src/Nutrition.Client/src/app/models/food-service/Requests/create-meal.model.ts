import { EatenDishModel } from "./eaten-dish.model";
import { EatenProductModel } from "./eaten-product.model";

export interface CreateMealModel {
  dayResultId: string,
  name: string | null,
  products: EatenProductModel[],
  dishes: EatenDishModel[]
}
