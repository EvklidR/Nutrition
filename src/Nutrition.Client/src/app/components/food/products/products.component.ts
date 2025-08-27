import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ProductService } from '../../../services/food-service/product.service';
import { GetFoodRequestParameters } from '../../../models/food-service/Requests/get-food-request-parameters.model';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';
import { AddProductModalComponent } from '../../modals/add-product-modal/add-product-modal.component';
import { faTrash, faEdit } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ConfirmDialogComponent } from '../../modals/confirm-dialog-modal/confirm-dialog.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProductResponse } from '../../../models/food-service/Responses/product.model';
import { ProductsResponse } from '../../../models/food-service/Responses/products.model';
import { FilterComponent } from '../filter/filter.component';
import { FoodCardComponent } from '../food-card/food-card.component';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    FontAwesomeModule,
    FilterComponent,
    FoodCardComponent
  ],
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css']
})
export class ProductsComponent implements OnInit {
  products: ProductResponse[] = [];
  totalCount: number = 0;

  params: GetFoodRequestParameters = {
    name: null,
    sortAsc: null,
    paginationParameters: {
      page: 1,
      pageSize: 10
    },
    sortingCriteria: null
  };

  isLoading: boolean = true;

  faTrash = faTrash;
  faEdit = faEdit;

  constructor(
    private router: Router,
    private productService: ProductService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.productService.getProducts(this.params).subscribe({
      next: (data: ProductsResponse) => {
        this.products = data.products;
        this.totalCount = data.totalCount;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Ошибка загрузки ингредиентов:', error);
        this.isLoading = false;
      }
    });
  }

  navigateToDishes(): void {
    this.router.navigate(['/dishes']);
  }

  deleteProduct(id: string): void {
    this.isLoading = true;
    this.productService.deleteProduct(id).subscribe({
      next: () => {
        this.loadProducts()
      },
      error: (error) => {
        console.error('Ошибка удаления ингредиента:', error);
        this.isLoading = false;
        if (error.error[0] == "This product there is in some dish") {
          this.sendPopUpNotification("Этот продукт содержится в блюде")
        }
        else {
          this.sendPopUpNotification("Не удалось удалить продукт")
        }
      }
    });
  }

  openAddProductModal() {
    const dialogRef = this.dialog.open(AddProductModalComponent, {});

    dialogRef.afterClosed().subscribe(() => {
      this.loadProducts();
    });
  }

  openEditProductModal(product: ProductResponse): void {
    const dialogRef = this.dialog.open(AddProductModalComponent, {
      data: product
    });

    dialogRef.afterClosed().subscribe(() => {
      this.loadProducts();
    });
  }

  openDeleteConfirmation(productId: string): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Удаление продукта',
        message: 'Вы уверены, что хотите удалить этот продукт?'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteProduct(productId);
      }
    });
  }

  sendPopUpNotification(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000
    });
  }
}
