import { Component, OnInit, Inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { DishService } from '../../../services/food-service/dish.service';
import { ProductService } from '../../../services/food-service/product.service';
import { MealService } from '../../../services/food-service/meal.service';
import { CreateMealModel } from '../../../models/food-service/Requests/create-meal.model';
import { GetFoodRequestParameters } from '../../../models/food-service/Requests/get-food-request-parameters.model';
import { ProductResponse } from '../../../models/food-service/Responses/product.model';
import { DishResponse } from '../../../models/food-service/Responses/dish.model';
import { UpdateMealModel } from '../../../models/food-service/Requests/update-meal.model';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-create-meal',
  standalone: true,
  imports: [
    FormsModule,
    CommonModule
  ],
  templateUrl: './create-meal.component.html',
  styleUrl: './create-meal.component.css'
})
export class CreateMealComponent implements OnInit {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { dayResultId: string, mealId: string | null },
    private dialogRef: MatDialogRef<CreateMealComponent>,
    private dishService: DishService,
    private productService: ProductService,
    private mealService: MealService
  )
  {
    if (data.mealId) {
      this.isEditMode = true
      this.editingMealId = data.mealId
    }
  }

  isDropdownVisible = false;
  isDishDropdownVisible = false;

  selectedProducts: { product: ProductResponse, weight: number }[] = [];
  selectedDishes: { dish: DishResponse, servings: number }[] = [];

  name: string = "";

  products: ProductResponse[] = [];
  dishes: DishResponse[] = [];

  mealToCreate: CreateMealModel | null = null;
  mealToUpdate: UpdateMealModel | null = null;

  isEditMode: boolean = false;
  editingMealId: string | null = null;

  ngOnInit() {
    forkJoin({
      products: this.productService.getProducts(this.getDefaultParams()),
      dishes: this.dishService.getDishes(this.getDefaultParams())
    }).subscribe(({ products, dishes }) => {
      this.products = products.products;
      this.dishes = dishes.dishes;

      if (this.isEditMode) {
        this.loadMealForEdit();
      } else {
        this.initMealForCreate();
      }
    });
  }

  private getDefaultParams(): GetFoodRequestParameters {
    return {
      name: null,
      paginationParameters: null,
      sortAsc: null,
      sortingCriteria: null
    };
  }

  private loadMealForEdit() {
    this.mealService.getMealById(this.data.mealId!, this.data.dayResultId).subscribe(meal => {
      this.mealToUpdate = this.initMealToUpdate(meal);

      this.name = meal.name ?? "";

      this.selectedDishes = meal.dishes.map(d => ({
        dish: this.dishes.find(dish => dish.id == d.id)!,
        servings: d.amountOfPortions
      }));

      this.selectedProducts = meal.products.map(p => ({
        product: this.products.find(product => product.id == p.id)!,
        weight: p.weight
      }));
    });
  }

  private initMealToUpdate(meal: any): UpdateMealModel {
    return {
      id: this.editingMealId!,
      dayResultId: this.data.dayResultId,
      name: meal.name ?? "",
      products: [],
      dishes: []
    };
  }

  private initMealForCreate() {
    this.mealToCreate = {
      dayResultId: this.data.dayResultId,
      name: "",
      products: [],
      dishes: []
    };

    this.selectedDishes = [];
    this.selectedProducts = [];
  }


  showProductsDropdown() {
    this.isDropdownVisible = true;
  }

  addProduct(product: ProductResponse) {
    this.selectedProducts.push({ product, weight: 100 });
    this.isDropdownVisible = false;
  }

  removeProduct(product: ProductResponse) {
    this.selectedProducts = this.selectedProducts.filter(item => item.product !== product);
  }

  showDishDropdown() {
    this.isDishDropdownVisible = true;
  }

  addDish(dish: DishResponse) {
    this.selectedDishes.push({ dish, servings: 1 });
    this.isDishDropdownVisible = false;
  }

  removeDish(dish: DishResponse) {
    this.selectedDishes = this.selectedDishes.filter(item => item.dish !== dish);
  }

  save() {
    if (this.selectedProducts.length === 0 && this.selectedDishes.length === 0) {
      this.dialogRef.close();
      return;
    }

    if (this.isEditMode) {
      for (const prod of this.selectedDishes) {
        this.mealToUpdate!.dishes.push({ foodId: prod.dish.id, amountOfPortions: prod.servings })
      }
      for (const prod of this.selectedProducts) {
        this.mealToUpdate!.products.push({ foodId: prod.product.id, weight: prod.weight })
      }

      this.mealToUpdate!.name = this.name

      this.mealService.updateMeal(this.mealToUpdate!).subscribe({
        next: (meal) => {
          console.log("meal was updated", meal)
          this.dialogRef.close();
        },
        error: (error) => {
          console.error("Error while updating meal:", error)
        }
      })
    }
    else {
      for (const prod of this.selectedDishes) {
        this.mealToCreate!.dishes.push({ foodId: prod.dish.id, amountOfPortions: prod.servings })
      }
      for (const prod of this.selectedProducts) {
        this.mealToCreate!.products.push({ foodId: prod.product.id, weight: prod.weight })
      }

      this.mealToCreate!.name = this.name

      this.mealService.createMeal(this.mealToCreate!).subscribe({
        next: (meal) => {
          console.log("meal was created", meal)
          this.dialogRef.close();
        },
        error: (error) => {
          console.error("Error while creating meal:", error)
        }
      })
    }
  }
}
