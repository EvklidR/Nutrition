import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GetFoodRequestParameters } from '../../models/food-service/Requests/get-food-request-parameters.model';
import { CalculatedRecipeResponse } from '../../models/food-service/Responses/calculated-recipe.model';
import { DishesResponse } from '../../models/food-service/Responses/dishes.model';
import { CreateRecipeModel } from '../../models/food-service/Requests/create-recipe.model';
import { UpdateRecipeModel } from '../../models/food-service/Requests/update-recipe.model';

@Injectable({
  providedIn: 'root'
})
export class DishService {
  private readonly baseUrl = 'https://localhost/food_service/dishes';

  constructor(private http: HttpClient) { }

  getRecipeById(recipeId: string): Observable<CalculatedRecipeResponse> {
    return this.http.get<CalculatedRecipeResponse>(`${this.baseUrl}/${recipeId}`);
  }

  getDishes(parameters: GetFoodRequestParameters): Observable<DishesResponse> {
    let params = new HttpParams();

    for (const key in parameters) {
      const value = (parameters as any)[key];
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

    return this.http.get<DishesResponse>(this.baseUrl, { params });
  }

  createRecipe(createRecipeDTO: CreateRecipeModel): Observable<CalculatedRecipeResponse> {
    const formData = new FormData();

    formData.append('name', createRecipeDTO.name);
    formData.append('amountOfPortions', createRecipeDTO.amountOfPortions.toString());

    if (createRecipeDTO.description) {
      formData.append('description', createRecipeDTO.description);
    }

    if (createRecipeDTO.image) {
      formData.append('image', createRecipeDTO.image);
    }

    createRecipeDTO.ingredients.forEach((ingredient, index) => {
      formData.append(`ingredients[${index}].productId`, ingredient.productId);
      formData.append(`ingredients[${index}].weightInRecipe`, ingredient.weightInRecipe.toString());
    });

    return this.http.post<CalculatedRecipeResponse>(this.baseUrl, formData);
  }

  updateRecipe(updateRecipeDTO: UpdateRecipeModel): Observable<void> {
    const formData = new FormData();

    formData.append("Id", updateRecipeDTO.id);
    formData.append("Name", updateRecipeDTO.name);
    formData.append("Description", updateRecipeDTO.description ?? "");
    formData.append("AmountOfPortions", updateRecipeDTO.amountOfPortions.toString());
    formData.append("DeleteImageIfNull", updateRecipeDTO.deleteImageIfNull.toString());

    if (updateRecipeDTO.image) {
      formData.append("Image", updateRecipeDTO.image);
    }

    updateRecipeDTO.ingredients.forEach((ingredient, index) => {
      formData.append(`Ingredients[${index}].ProductId`, ingredient.productId);
      formData.append(`Ingredients[${index}].WeightInRecipe`, ingredient.weightInRecipe.toString());
    });

    return this.http.put<void>(this.baseUrl, formData);
  }

  deleteRecipe(recipeId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${recipeId}`);
  }

  getRecipeImage(recipeId: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/${recipeId}/image`, { responseType: 'blob' });
  }
}
