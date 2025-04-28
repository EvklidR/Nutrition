import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FullDishModel } from '../../models/food-service/Responces/full-dish.model';
import { DishesResponseModel } from '../../models/food-service/Responces/dishes.model';
import { CreateDishModel } from '../../models/food-service/Requests/create-dish.model';
import { UpdateDishModel } from '../../models/food-service/Requests/update-dish.model';
import { GetFoodRequestParameters } from '../../models/food-service/Requests/get-food-request-parameters.model';

@Injectable({
  providedIn: 'root'
})
export class DishService {
  private readonly baseUrl = 'https://localhost/food_service/dishes';

  constructor(private http: HttpClient) { }

  getDishById(dishId: string): Observable<FullDishModel> {
    return this.http.get<FullDishModel>(`${this.baseUrl}/${dishId}`);
  }

  getDishes(parameters: GetFoodRequestParameters): Observable<DishesResponseModel> {
    let params = new HttpParams();

    for (const key in parameters) {
      const value = (parameters as any)[key];
      if (value !== null && value !== undefined) {
        params = params.set(key, value.toString());
      }
    }

    return this.http.get<DishesResponseModel>(this.baseUrl, { params });
  }

  createDish(createDishDTO: CreateDishModel): Observable<FullDishModel> {
    const formData = new FormData();

    if (createDishDTO.userId) {
      formData.append('userId', createDishDTO.userId);
    }

    formData.append('name', createDishDTO.name);
    formData.append('amountOfPortions', createDishDTO.amountOfPortions.toString());

    if (createDishDTO.description) {
      formData.append('description', createDishDTO.description);
    }

    if (createDishDTO.image) {
      formData.append('image', createDishDTO.image);
    }

    createDishDTO.ingredients.forEach((ingredient, index) => {
      formData.append(`ingredients[${index}].productId`, ingredient.productId);
      formData.append(`ingredients[${index}].weight`, ingredient.weight.toString());
    });

    return this.http.post<FullDishModel>(this.baseUrl, formData);
  }


  updateDish(updateDishDTO: UpdateDishModel): Observable<void> {
    return this.http.put<void>(this.baseUrl, updateDishDTO);
  }

  deleteDish(dishId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${dishId}`);
  }

  getDishImage(dishId: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/${dishId}/image`, { responseType: 'blob' });
  }
}
