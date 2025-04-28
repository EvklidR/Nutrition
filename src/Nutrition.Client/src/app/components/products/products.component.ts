import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { UserService } from '../../services/user-service/user.service';
import { ProductService } from '../../services/food-service/product.service';
import { GetFoodRequestParameters } from '../../models/food-service/Requests/get-food-request-parameters.model';
import { CommonModule } from '@angular/common';
import { ProductResponseModel } from '../../models/food-service/Responces/product.model';
import { ProductsResponseModel } from '../../models/food-service/Responces/products.model';
import { MatDialog } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';
import { AddProductModalComponent } from '../modals/add-product-modal/add-product-modal.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    FontAwesomeModule
  ],
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css']
})
export class ProductsComponent implements OnInit {
  products: ProductResponseModel[] = [];
  totalCount: number = 0;

  params: GetFoodRequestParameters = {
    name: null,
    page: 1,
    pageSize: 10,
    sortAsc: null,
    sortingCriteria: null
  }

  hasNextPage: boolean = false;
  isLoading: boolean = true;

  constructor(
    private router: Router,
    private userService: UserService,
    private productService: ProductService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    if (this.params.sortingCriteria == null) {
      this.params.sortAsc = null
    }

    this.productService.getProducts(this.params).subscribe({
      next: (data: ProductsResponseModel) => {
        this.products = data.products;
        this.totalCount = data.totalCount;
        this.isLoading = false;

        const totalPages = this.totalPages()
        this.hasNextPage = (this.params.page || 1) < totalPages;
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
      }
    });
  }

  openAddProductModal() {
    const dialogRef = this.dialog.open(AddProductModalComponent, {
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadProducts()
      console.log('Диалог закрыт');
    });
  }

  changePage(offset: number): void {
    if (!this.params.page) this.params.page = 1;
    this.params.page += offset;
    this.loadProducts();
  }

  totalPages(): number {
    return Math.ceil(this.totalCount / (this.params.pageSize || 1));
  }
}
