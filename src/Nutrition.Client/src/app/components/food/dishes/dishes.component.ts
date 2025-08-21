import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { UserService } from '../../../services/user-service/user.service';
import { DishService } from '../../../services/food-service/dish.service';
import { ProfileService } from '../../../services/user-service/profile.service';
import { GetFoodRequestParameters } from '../../../models/food-service/Requests/get-food-request-parameters.model';
import { MatDialog } from '@angular/material/dialog';
import { DishDetailsModalComponent } from '../../modals/recipe-details-modal/recipe-details-modal.component';
import { AddDishModalComponent } from '../../modals/add-dish-modal/add-dish-modal.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faTrash } from '@fortawesome/free-solid-svg-icons';
import { ConfirmDialogComponent } from '../../modals/confirm-dialog-modal/confirm-dialog.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DishResponse } from '../../../models/food-service/Responses/dish.model';
import { DishesResponse } from '../../../models/food-service/Responses/dishes.model';
import { FilterComponent } from '../filter/filter.component';
import { FoodCardComponent } from '../food-card/food-card.component';

@Component({
  selector: 'app-dishes',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    FontAwesomeModule,
    FilterComponent,
    FoodCardComponent
  ],
  templateUrl: './dishes.component.html',
  styleUrls: ['./dishes.component.css']
})
export class DishesComponent implements OnInit {
  dishImages: { [dishId: string]: string } = {};
  dishes: DishResponse[] = [];
  totalCount: number = 0;

  params!: GetFoodRequestParameters

  hasNextPage: boolean = false;

  isLoading: boolean = true;

  faTrash = faTrash;

  constructor(
    private router: Router,
    private userService: UserService,
    private dishService: DishService,
    private profileService: ProfileService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.loadDishes();
  }

  loadDishes(): void {
    this.dishService.getDishes(this.params).subscribe({
      next: (data: DishesResponse) => {
        this.dishes = data.dishes;
        this.totalCount = data.totalCount;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Ошибка загрузки блюд:', error);
        this.isLoading = false;
      }
    });
  }

  navigateToProducts(): void {
    this.router.navigate(['/products']);
  }

  deleteDish(recipeId: string): void {
    this.isLoading = true;
    this.dishService.deleteRecipe(recipeId).subscribe({
      next: () => {
        this.loadDishes()
      },
      error: (error) => {
        console.error('Ошибка удаления блюда:', error);
        this.isLoading = false;
        if (error.error[0] == "This dish is used in some meals") {
          this.sendPopUpNotification("Это блюдо есть в приемах пищи")
        }
        else {
          this.sendPopUpNotification("Не удалось удалить продукт")
        }
      }
    });
  }

  openDatailsModal(dishId: string) {
    const dialogRef = this.dialog.open(DishDetailsModalComponent, {
      width: '600px',
      maxWidth: '600px',
      minWidth: '500px',
      panelClass: 'no-scroll-dialog',
      data: { dishId: dishId }
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('Диалог закрыт');
    });
  }

  openAddDishModal() {
    const dialogRef = this.dialog.open(AddDishModalComponent, {
      width: '800px',
      maxWidth: '800px',
      minWidth: '500px'
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadDishes()
      console.log('Диалог закрыт');
    });
  }

  openEditDishModal(recipeId: string) {
    const dialogRef = this.dialog.open(AddDishModalComponent, {
      width: '800px',
      maxWidth: '800px',
      minWidth: '500px',
      data: {
        recipeId: recipeId
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadDishes()
      console.log('Диалог закрыт');
    });
  }

  openDeleteConfirmation(recipeId: string): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Удаление блюда',
        message: 'Вы уверены, что хотите удалить это блюдо?'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteDish(recipeId);
      }
    });
  }

  sendPopUpNotification(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000
    });
    return
  }
}
