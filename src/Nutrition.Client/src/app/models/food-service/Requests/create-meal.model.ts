import { EatenFoodModel } from "./eaten-food.model";

export interface CreateMealModel {
  dayId: string,
  name: string,
  products: EatenFoodModel[],
  dishes: EatenFoodModel[]
}
