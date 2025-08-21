import { Component, Inject } from '@angular/core';
import { DishService } from '../../../services/food-service/dish.service';
import { ProductService } from '../../../services/food-service/product.service';
import { GetFoodRequestParameters } from '../../../models/food-service/Requests/get-food-request-parameters.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProductsResponse } from '../../../models/food-service/Responses/products.model';
import { ProductOfRecipeModel } from '../../../models/food-service/Requests/product-of-recipe.model';
import { ProductResponse } from '../../../models/food-service/Responses/product.model';
import { CreateRecipeModel } from '../../../models/food-service/Requests/create-recipe.model';
import { UpdateRecipeModel } from '../../../models/food-service/Requests/update-recipe.model';


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
  description: string | null = null;
  amountOfPortions!: number;
  imageUrl: string | null = null;
  selectedImage: File | null = null;

  currentImage: File | null = null;

  allProducts: ProductResponse[] = [];
  selectedProducts: ProductOfRecipeModel[] = [];
  selectedProductId: string | null = null;

  isSaving: boolean = false;

  recipeId: string | null = null;
  isEditingMode: boolean = false;

  constructor(
    private dishService: DishService,
    private productService: ProductService,
    private dialogRef: MatDialogRef<AddDishModalComponent>,
    private snackBar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public data: { recipeId: string | null},
  ) {
    if (data?.recipeId ?? false) {
      this.isEditingMode = true;
      this.recipeId = data.recipeId;
    }
  }

  ngOnInit(): void {
    const params: GetFoodRequestParameters = {
      name: null,
      paginatedParameters: null,
      sortAsc: null,
      sortingCriteria: null
    }

    this.productService.getProducts(params).subscribe({
      next: (data: ProductsResponse) => {
        this.allProducts = data.products;

        if (this.isEditingMode) {
          this.dishService.getRecipeById(this.recipeId!).subscribe(
            (recipe) => {
              this.name = recipe.name
              this.description = recipe.description
              this.amountOfPortions = recipe.amountOfPortions
              this.imageUrl = recipe.imageUrl
              this.selectedProducts = recipe.ingredients.map(i => ({
                productId: i.id,
                weightInRecipe: i.weight
              }))
            }
          )
          this.loadImage(this.recipeId!)
        }
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
      weightInRecipe: 50
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

    if (this.isEditingMode) {
      this.updateRecipe()
    }
    else {
      this.createRecipe()
    }

  }

  createRecipe() {
    const newDish: CreateRecipeModel = {
      name: this.name,
      description: this.description || '',
      amountOfPortions: this.amountOfPortions,
      ingredients: this.selectedProducts,
      image: this.selectedImage
    };

    this.isSaving = true;

    this.dishService.createRecipe(newDish).subscribe({
      next: (dish) => {
        this.dialogRef.close();
        this.isSaving = false;
      },
      error: (error) => {
        console.error('Ошибка при создании блюда:', error);
        this.isSaving = false;
      }
    });
  }

  updateRecipe() {
    const recipe: UpdateRecipeModel = {
      id: this.recipeId!,
      name: this.name,
      description: this.description || '',
      amountOfPortions: this.amountOfPortions,
      ingredients: this.selectedProducts,
      image: this.selectedImage,
      deleteImageIfNull: !!this.selectedImage
    };

    this.isSaving = true;

    this.dishService.updateRecipe(recipe).subscribe({
      next: () => {
        this.dialogRef.close();
        this.isSaving = false;
      },
      error: (error) => {
        console.error('Ошибка при изменении блюда:', error);
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

  loadImage(recipeId: string): void {
    this.dishService.getRecipeImage(recipeId).subscribe({
      next: (blob) => {
        const file = new File([blob], `recipe-${recipeId}.png`, { type: blob.type });
        this.currentImage = file;
        this.imageUrl = URL.createObjectURL(file);
      },
      error: (error) => {
        console.error(`Ошибка загрузки изображения для блюда с ID ${recipeId}:`, error);
      }
    });
  }
}
