import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { CreateProductModel } from '../../models/food-service/Requests/create-product.model';
import { UpdateProductModel } from '../../models/food-service/Requests/update-product.model';
import { GetFoodRequestParameters } from '../../models/food-service/Requests/get-food-request-parameters.model';
import { ProductsResponse } from '../../models/food-service/Responses/products.model';
import { ProductResponseFromAPI } from '../../models/food-service/Responses/product-from-api.model';
import { ProductResponse } from '../../models/food-service/Responses/product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private readonly baseUrl = 'https://localhost/food_service/products';

  constructor(private http: HttpClient) { }

  getProducts(paramsObj: GetFoodRequestParameters): Observable<ProductsResponse> {
    let params = new HttpParams();

    for (const key in paramsObj) {
      const value = (paramsObj as any)[key];
      if (value !== null && value !== undefined) {
        if (typeof value === 'object' && !Array.isArray(value)) {
          for (const subKey in value) {
            const subValue = (value as any)[subKey];
            if (subValue !== null && subValue !== undefined) {
              params = params.set(key + "." + subKey, subValue.toString());
            }
          }
        } else {
          params = params.set(key, value.toString());
        }
      }
    }

    return this.http.get<ProductsResponse>(this.baseUrl, { params });
  }

  searchProductByName(name: string): Observable<ProductResponseFromAPI[]> {
    return this.http.get<ProductResponseFromAPI[]>(`${this.baseUrl}/search-product/${encodeURIComponent(name)}`);
  }

  createProduct(productData: CreateProductModel): Observable<ProductResponse> {
    return this.http.post<ProductResponse>(this.baseUrl, productData);
  }

  updateProduct(productData: UpdateProductModel): Observable<void> {
    return this.http.put<void>(this.baseUrl, productData);
  }

  deleteProduct(productId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${productId}`);
  }
}
