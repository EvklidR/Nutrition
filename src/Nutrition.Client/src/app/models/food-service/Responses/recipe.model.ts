import { RecipeProductResponse } from "./recipe-product.model"

export interface RecipeResponse {
  id: string
  name: string
  description: string | null
  imageUrl: string | null
  amountOfPortions: number

  ingredients: RecipeProductResponse[]
}
