import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ProductService } from '../../../services/food-service/product.service';
import { CreateProductModel } from '../../../models/food-service/Requests/create-product.model';
import { ProductResponseModel } from '../../../models/food-service/Responces/product.model';
import { ProductResponseFromAPIModel } from '../../../models/food-service/Responces/product-from-api.model';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UpdateProductModel } from '../../../models/food-service/Requests/update-product.model';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-add-ingredient-modal',
  standalone: true,
  imports: [FormsModule, CommonModule],
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

  isEditMode: boolean = false;
  editingProductId: string | null = null;

  constructor(
    private productService: ProductService,
    private dialogRef: MatDialogRef<AddProductModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ProductResponseModel | null,
    private snackBar: MatSnackBar
  ) {
    if (data) {
      this.isEditMode = true;
      this.editingProductId = data.id;
      this.name = data.name;
      this.proteins = data.proteins;
      this.fats = data.fats;
      this.carbohydrates = data.carbohydrates;
    }

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
      next: (data) => (this.productsFromApi = data),
      error: () => (this.productsFromApi = [])
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
      this.sendPopUpNotification('Пожалуйста, заполните все поля.');
      return;
    }

    this.isSaving = true;

    if (this.isEditMode && this.editingProductId) {
      const updatePayload: UpdateProductModel = {
        id: this.editingProductId,
        name: this.name,
        proteins: this.proteins,
        fats: this.fats,
        carbohydrates: this.carbohydrates
      };

      this.productService.updateProduct(updatePayload).subscribe({
        next: () => this.dialogRef.close(),
        error: () => (this.isSaving = false)
      });
    } else {
      const createPayload: CreateProductModel = {
        userId: null,
        name: this.name,
        proteins: this.proteins,
        fats: this.fats,
        carbohydrates: this.carbohydrates
      };

      this.productService.createProduct(createPayload).subscribe({
        next: () => this.dialogRef.close(),
        error: () => (this.isSaving = false)
      });
    }
  }

  sendPopUpNotification(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000
    });
    return
  }
}
