import { Component, EventEmitter, Output, Input } from '@angular/core';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { ProductResponseFromAPIModel } from '../../../models/food-service/Responces/product-from-api.model';
import { ProductService } from '../../../services/food-service/product.service';
import { CreateProductModel } from '../../../models/food-service/Requests/create-product.model';
import { ProductResponseModel } from '../../../models/food-service/Responces/product.model';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-add-ingredient-modal',
  standalone: true,
  imports: [
    FormsModule,
    CommonModule
  ],
  templateUrl: './add-product-modal.component.html',
  styleUrls: ['./add-product-modal.component.css'],
})
export class AddProductModalComponent {

  name: string = '';
  proteins: number | null = null;
  fats: number | null = null;
  carbohydrates: number | null = null;

  isSaving: boolean = false;

  productSearchTerm: string = '';
  productsFromApi: ProductResponseFromAPIModel[] = [];
  searchDebounce: Subject<string> = new Subject<string>();

  constructor(
    private productService: ProductService,
    private dialogRef: MatDialogRef<AddProductModalComponent>) {
    this.searchDebounce.pipe(debounceTime(1000)).subscribe((searchTerm) => {
      this.getProductsFromApi(searchTerm);
    });
  }

  onSearchChange(): void {
    this.searchDebounce.next(this.productSearchTerm);
  }

  getProductsFromApi(searchTerm: string): void {
    if (!searchTerm.trim()) {
      this.productsFromApi = [];
      return;
    }
    this.productService.searchProductByName(searchTerm).subscribe({
      next: (data) => {
        console.log(data)
        this.productsFromApi = data;
      },
      error: (error) => {
        console.error('Ошибка получения ингредиентов:', error);
        this.productsFromApi = [];
      }
    });
  }

  selectProduct(product: ProductResponseFromAPIModel): void {
    this.name = product.name;
    this.proteins = product.proteins;
    this.fats = product.fats;
    this.carbohydrates = product.carbohydrates;

    this.productSearchTerm = '';
    this.productsFromApi = [];
  }

  saveProduct(): void {
    if (!this.name || this.proteins === null || this.fats === null || this.carbohydrates === null) {
      alert('Пожалуйста, заполните все поля.');
      return;
    }

    const newIngredient: CreateProductModel = {
      userId: null,
      name: this.name,
      proteins: this.proteins,
      fats: this.fats,
      carbohydrates: this.carbohydrates,
    };

    this.isSaving = true;

    this.productService.createProduct(newIngredient).subscribe({
      next: (createdIngredient: ProductResponseModel) => {
        this.isSaving = false;
        this.dialogRef.close()
      },
      error: (error) => {
        console.error('Ошибка при сохранении ингредиента:', error);
        this.isSaving = false;
      }
    });
  }
}
