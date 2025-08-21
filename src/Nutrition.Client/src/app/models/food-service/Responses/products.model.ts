import { ProductResponse } from "./product.model";

export interface ProductsResponse {
  products: ProductResponse[],
  totalCount: number
}
