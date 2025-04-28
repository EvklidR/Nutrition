import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { FullMealModel } from '../../../models/food-service/Responces/full-meal.model';
import { MealService } from '../../../services/food-service/meal.service';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

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
    private mealService: MealService
  ) { }

  meal!: FullMealModel;

  ngOnInit(): void {
    this.mealService.getMealById(this.data.mealId, this.data.dayId).subscribe({
      next: (mealResponce) => {
        this.meal = mealResponce
      },
      error: (error) => {
        console.log(error)
      }
    })
  }
}
