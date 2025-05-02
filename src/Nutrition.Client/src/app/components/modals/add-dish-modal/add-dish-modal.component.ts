import { Component, EventEmitter, Output, Input } from '@angular/core';
import { ProductResponseModel } from '../../../models/food-service/Responces/product.model';
import { ProductOfDishModel } from '../../../models/food-service/Requests/product-of-dish.model';
import { DishService } from '../../../services/food-service/dish.service';
import { ProductService } from '../../../services/food-service/product.service';
import { GetFoodRequestParameters } from '../../../models/food-service/Requests/get-food-request-parameters.model';
import { ProductsResponseModel } from '../../../models/food-service/Responces/products.model';
import { CreateDishModel } from '../../../models/food-service/Requests/create-dish.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';


@Component({
  selector: 'app-add-dish-modal',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './add-dish-modal.component.html',
  styleUrls: ['./add-dish-modal.component.css']
})
export class AddDishModalComponent {
  name: string = '';
  proteins: number | null = null;
  fats: number | null = null;
  carbohydrates: number | null = null;
  description?: string;
  amountOfPortions!: number;
  imageUrl?: string;
  selectedImage: File | null = null;

  allProducts: ProductResponseModel[] = [];
  selectedProducts: ProductOfDishModel[] = [];
  selectedProductId: string | null = null;

  isSaving: boolean = false;

  constructor(
    private dishService: DishService,
    private productService: ProductService,
    private dialogRef: MatDialogRef<AddDishModalComponent>,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    const params: GetFoodRequestParameters = {
      name: null,
      page: null,
      pageSize: null,
      sortAsc: null,
      sortingCriteria: null
    }

    this.productService.getProducts(params).subscribe({
      next: (data: ProductsResponseModel) => {
        this.allProducts = data.products;
      },
      error: (error) => {
        console.error('Ошибка загрузки ингредиентов:', error);
      }
    });
  }

  addProduct(): void {
    if (!this.selectedProductId) return;

    const exists = this.selectedProducts.some(prod => prod.productId === this.selectedProductId);
    if (exists) {
      alert('Этот ингредиент уже добавлен.');
      return;
    }

    this.selectedProducts.push({
      productId: this.selectedProductId,
      weight: 50
    });

    this.selectedProductId = null;
  }

  removeProduct(productId: string): void {
    this.selectedProducts = this.selectedProducts.filter(prod => prod.productId !== productId);
  }

  getProductName(ingredientId: string): string {
    const ingredient = this.allProducts.find(i => i.id === ingredientId);
    return ingredient ? ingredient.name : 'Неизвестный ингредиент';
  }

  onImageChange(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedImage = file;
      this.imageUrl = URL.createObjectURL(file);
    }
  }

  saveDish(): void {
    if (!this.name || !this.amountOfPortions || this.selectedProducts.length == 0) {
      this.sendPopUpNotification("Заполните все обязательные поля!")
      return;
    }

    const newDish: CreateDishModel = {
      userId: null,
      name: this.name,
      description: this.description || '',
      amountOfPortions: this.amountOfPortions,
      ingredients: this.selectedProducts,
      image: this.selectedImage
    };

    this.isSaving = true;

    this.dishService.createDish(newDish).subscribe({
      next: (dish) => {
        this.dialogRef.close();
        this.isSaving = false;
      },
      error: (error) => {
        console.error('Ошибка при сохранении блюда:', error);
        this.isSaving = false;
      }
    });
  }

  close(): void {
    this.dialogRef.close();
  }

  sendPopUpNotification(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000
    });
    return
  }
}
