import { Component, EventEmitter, Output, Input, OnInit, Inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { DishService } from '../../../services/food-service/dish.service';
import { ProductService } from '../../../services/food-service/product.service';
import { MealService } from '../../../services/food-service/meal.service';
import { CreateMealModel } from '../../../models/food-service/Requests/create-meal.model';
import { DishesResponseModel } from '../../../models/food-service/Responces/dishes.model';
import { ProductsResponseModel } from '../../../models/food-service/Responces/products.model';
import { ProductResponseModel } from '../../../models/food-service/Responces/product.model';
import { BriefDishModel } from '../../../models/food-service/Responces/brief-dish.model';
import { GetFoodRequestParameters } from '../../../models/food-service/Requests/get-food-request-parameters.model';

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
    @Inject(MAT_DIALOG_DATA) public data: { dayResultId: string },
    private dialogRef: MatDialogRef<CreateMealComponent>,
    private dishService: DishService,
    private productService: ProductService,
    private snackBar: MatSnackBar,
    private mealService: MealService
  ) { }

  isDropdownVisible = false;
  isDishDropdownVisible = false;

  selectedProducts: { product: ProductResponseModel, weight: number }[] = [];
  selectedDishes: { dish: BriefDishModel, servings: number }[] = [];

  products: ProductResponseModel[] = [];
  dishes: BriefDishModel[] = [];

  mealToCreate: CreateMealModel = {
    dayId: "",
    name: "",
    products: [],
    dishes: []
  };

  ngOnInit() {
    this.loadProducts();
    this.loadDishes();
    this.selectedDishes = []
    this.selectedProducts = []
  }

  loadProducts() {
    const params: GetFoodRequestParameters = {
      name: null,
      page: null,
      pageSize: null,
      sortAsc: null,
      sortingCriteria: null
    }

    this.productService.getProducts(params).subscribe({
      next: (products) => {
        this.products = products.products;
      },
      error: (error) => {
        console.error('Ошибка при загрузке ингредиентов', error);
      }
    });
  }

  loadDishes() {
    const params: GetFoodRequestParameters = {
      name: null,
      page: null,
      pageSize: null,
      sortAsc: null,
      sortingCriteria: null
    }

    this.dishService.getDishes(params).subscribe({
      next: (dishes) => {
        this.dishes = dishes.dishes;
      },
      error: (error) => {
        console.error('Ошибка при загрузке блюд', error);
      }
    });
  }

  showProductsDropdown() {
    this.isDropdownVisible = true;
  }

  addProduct(product: ProductResponseModel) {
    this.selectedProducts.push({ product, weight: 100 });
    this.isDropdownVisible = false;
  }

  removeProduct(product: ProductResponseModel) {
    this.selectedProducts = this.selectedProducts.filter(item => item.product !== product);
  }

  showDishDropdown() {
    this.isDishDropdownVisible = true;
  }

  addDish(dish: BriefDishModel) {
    this.selectedDishes.push({ dish, servings: 1 });
    this.isDishDropdownVisible = false;
  }

  removeDish(dish: BriefDishModel) {
    this.selectedDishes = this.selectedDishes.filter(item => item.dish !== dish);
  }

  addMeal() {
    if (this.selectedProducts.length === 0 && this.selectedDishes.length === 0) {
      this.dialogRef.close();
      return;
    }

    if (!this.mealToCreate.name || this.mealToCreate.name.trim() === '') {
      this.snackBar.open('Meal name cannot be empty!', 'Close', {
        duration: 3000,
        verticalPosition: 'bottom',
        horizontalPosition: 'center',
        panelClass: ['snackbar-error']
      });
      return;
    }

    for (const prod of this.selectedDishes) {
      this.mealToCreate.dishes.push({ foodId: prod.dish.id, weight: prod.servings * prod.dish.weight })
    }
    for (const prod of this.selectedProducts) {
      this.mealToCreate.products.push({ foodId: prod.product.id, weight: prod.weight })
    }

    this.mealToCreate.dayId = this.data.dayResultId

    this.mealService.createMeal(this.mealToCreate).subscribe({
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
