import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MealService } from '../../../services/food-service/meal.service';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FullMealResponse } from '../../../models/food-service/Responses/full-meal.model';
import { CreateMealComponent } from '../create-meal-modal/create-meal.component';

@Component({
  selector: 'app-meal-details-modal',
  standalone: true,
  imports: [
    CommonModule,
    FontAwesomeModule
  ],
  templateUrl: './meal-details-modal.component.html',
  styleUrls: ['./meal-details-modal.component.css']
})
export class MealDetailsModalComponent implements OnInit {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { mealId: string, dayId: string },
    private mealService: MealService,
    private dialog: MatDialog,
  ) { }

  meal!: FullMealResponse;

  ngOnInit(): void {
    this.mealService.getMealById(this.data.mealId, this.data.dayId).subscribe({
      next: (mealResponse) => {
        this.meal = mealResponse
      },
      error: (error) => {
        console.log(error)
      }
    })
  }

  openUpdatingMealModal(): void {
    const dialogRef = this.dialog.open(CreateMealComponent, {
      width: '900px',
      maxWidth: '900px',
      minWidth: '830px',
      height: '500px',
      data: { dayResultId: this.data.dayId, mealId: this.meal.id }
    });

    dialogRef.afterClosed().subscribe(() => {
      this.mealService.getMealById(this.data.mealId, this.data.dayId).subscribe({
        next: (mealResponse) => {
          this.meal = mealResponse
        },
        error: (error) => {
          console.log(error)
        }
      })
      console.log('Диалог закрыт');
    });
  }
}
