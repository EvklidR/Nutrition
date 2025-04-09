import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { CreateProductModel } from '../../models/food-service/Requests/create-product.model';
import { UpdateProductModel } from '../../models/food-service/Requests/update-product.model';
import { GetFoodRequestParameters } from '../../models/food-service/Requests/get-food-request-parameters.model';
import { ProductResponseModel } from '../../models/food-service/Responces/product.model';
import { ProductsResponseModel } from '../../models/food-service/Responces/products.model';
import { ProductResponseFromAPIModel } from '../../models/food-service/Responces/product-from-api.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private readonly baseUrl = 'https://localhost/food_service/products';

  constructor(private http: HttpClient) { }

  getProducts(paramsObj: GetFoodRequestParameters): Observable<ProductsResponseModel> {
    let params = new HttpParams();

    for (const key in paramsObj) {
      const value = (paramsObj as any)[key];
      if (value !== null && value !== undefined) {
        params = params.set(key, value.toString());
      }
    }

    return this.http.get<ProductsResponseModel>(this.baseUrl, { params });
  }

  searchProductByName(name: string): Observable<ProductResponseFromAPIModel[]> {
    return this.http.get<ProductResponseFromAPIModel[]>(`${this.baseUrl}/search-product/${encodeURIComponent(name)}`);
  }

  createProduct(productData: CreateProductModel): Observable<ProductResponseModel> {
    return this.http.post<ProductResponseModel>(this.baseUrl, productData);
  }

  updateProduct(productData: UpdateProductModel): Observable<void> {
    return this.http.put<void>(this.baseUrl, productData);
  }

  deleteProduct(productId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}?productId=${productId}`);
  }
}
