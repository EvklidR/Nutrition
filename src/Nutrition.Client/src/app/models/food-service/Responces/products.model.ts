import { ProductResponseModel } from "./product.model";

export interface ProductsResponseModel {
  products: ProductResponseModel[],
  totalCount: number
}
