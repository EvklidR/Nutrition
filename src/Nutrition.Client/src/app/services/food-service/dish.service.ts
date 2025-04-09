import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FullDishModel } from '../../models/food-service/Responces/full-dish.model';
import { DishesResponseModel } from '../../models/food-service/Responces/dishes.model';
import { CreateDishModel } from '../../models/food-service/Requests/create-dish.model';
import { UpdateDishModel } from '../../models/food-service/Requests/update-dish.model';

@Injectable({
  providedIn: 'root'
})
export class DishService {
  private readonly baseUrl = 'https://localhost/food_service/dishes';

  constructor(private http: HttpClient) { }

  getDishById(dishId: string): Observable<FullDishModel> {
    return this.http.get<FullDishModel>(`${this.baseUrl}/${dishId}`);
  }

  getDishes(parameters: any): Observable<DishesResponseModel> {
    let params = new HttpParams();

    if (parameters.page != null) {
      params = params.set('page', parameters.page);
    }

    if (parameters.size != null) {
      params = params.set('size', parameters.size);
    }

    return this.http.get<DishesResponseModel>(this.baseUrl, { params });
  }

  createDish(createDishDTO: CreateDishModel): Observable<FullDishModel> {
    return this.http.post<FullDishModel>(this.baseUrl, createDishDTO);
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
