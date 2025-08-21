import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DishService } from '../../../services/food-service/dish.service';
import { CommonModule } from '@angular/common';

import { CalculatedRecipeResponse } from '../../../models/food-service/Responses/calculated-recipe.model';

@Component({
  selector: 'app-dish-datails-modal',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './recipe-details-modal.component.html',
  styleUrls: ['./recipe-details-modal.component.css']
})
export class DishDetailsModalComponent implements OnInit {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { dishId: string },
    private dishService: DishService
  ) { }

  recipe!: CalculatedRecipeResponse;

  ngOnInit(): void {
    this.dishService.getRecipeById(this.data.dishId).subscribe({
      next: (recipeResponse: CalculatedRecipeResponse) => {
        this.recipe = recipeResponse
        console.log("получено блюдо: ", recipeResponse)
      },
      error: (error) => {
        console.log(error)
      }
    })
  }
}
