import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';

import { ProfileModel } from '../../models/user-service/Models/profile.model';
import { UserService } from '../../services/user-service/user.service';
import { DishService } from '../../services/food-service/dish.service';
import { ProfileService } from '../../services/user-service/profile.service';
import { BriefDishModel } from '../../models/food-service/Responces/brief-dish.model';
import { GetFoodRequestParameters } from '../../models/food-service/Requests/get-food-request-parameters.model';
import { DishesResponseModel } from '../../models/food-service/Responces/dishes.model';
import { MatDialog } from '@angular/material/dialog';
import { DishDetailsModalComponent } from '../modals/dish-details-modal/dish-details-modal.component';
import { AddDishModalComponent } from '../modals/add-dish-modal/add-dish-modal.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

@Component({
  selector: 'app-dishes',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    FontAwesomeModule
  ],
  templateUrl: './dishes.component.html',
  styleUrls: ['./dishes.component.css']
})
export class DishesComponent implements OnInit {
  dishImages: { [dishId: string]: string } = {};
  dishes: BriefDishModel[] = [];
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
    private dishService: DishService,
    private profileService: ProfileService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadDishes();
  }

  loadDishes(): void {
    if (this.params.sortingCriteria == null) {
      this.params.sortAsc = null
    }

    this.dishService.getDishes(this.params).subscribe({
      next: (data: DishesResponseModel) => {
        this.dishes = data.dishes;
        this.totalCount = data.totalCount;
        this.isLoading = false;

        const totalPages = this.totalPages()
        this.hasNextPage = (this.params.page || 1) < totalPages;

        this.dishes.forEach(dish => {
          if (dish.imageUrl && !this.dishImages[dish.id]) {
            this.loadImage(dish.id);
          }
        });
      },
      error: (error) => {
        console.error('Ошибка загрузки блюд:', error);
        this.isLoading = false;
      }
    });
  }

  loadImage(dishId: string): void {
    this.dishService.getDishImage(dishId).subscribe({
      next: (blob) => {
        const reader = new FileReader();
        reader.onload = () => {
          this.dishImages[dishId] = reader.result as string;
        };
        reader.readAsDataURL(blob);
      },
      error: (error) => {
        console.error(`Ошибка загрузки изображения для блюда с ID ${dishId}:`, error);
      }
    });
  }

  navigateToProducts(): void {
    this.router.navigate(['/products']);
  }

  deleteDish(id: string): void {
    this.isLoading = true;
    this.dishService.deleteDish(id).subscribe({
      next: () => {
        this.loadDishes()
      },
      error: (error) => {
        console.error('Ошибка удаления блюда:', error);
        this.isLoading = false;
      }
    });
  }

  openDatailsModal(dishId: string) {
    const dialogRef = this.dialog.open(DishDetailsModalComponent, {
      width: '600px',
      maxWidth: '600px',
      minWidth: '500px',
      data: { dishId: dishId }
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('Диалог закрыт');
    });
  }

  openAddDishModal() {
    const dialogRef = this.dialog.open(AddDishModalComponent, {
      width: '600px',
      maxWidth: '600px',
      minWidth: '500px',
      height: '500px'
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadDishes()
      console.log('Диалог закрыт');
    });
  }

  changePage(offset: number): void {
    if (!this.params.page) this.params.page = 1;
    this.params.page += offset;
    this.loadDishes();
  }

  totalPages(): number {
    return Math.ceil(this.totalCount / (this.params.pageSize || 10));
  }
}
