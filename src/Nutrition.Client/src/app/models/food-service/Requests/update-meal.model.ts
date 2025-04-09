import { EatenFoodModel } from "./eaten-food.model";

export interface UpdateMealModel {
  id: string,
  dayId: string,
  name: string,
  products: EatenFoodModel[],
  dishes: EatenFoodModel[]
}
