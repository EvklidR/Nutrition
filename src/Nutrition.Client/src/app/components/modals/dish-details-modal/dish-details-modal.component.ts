import { Component, Input, Output, EventEmitter, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DishService } from '../../../services/food-service/dish.service';
import { FullDishModel } from '../../../models/food-service/Responces/full-dish.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dish-datails-modal',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './dish-details-modal.component.html',
  styleUrls: ['./dish-details-modal.component.css']
})
export class DishDetailsModalComponent implements OnInit {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { dishId: string },
    private dishService: DishService
  ) { }

  dish!: FullDishModel;

  ngOnInit(): void {
    this.dishService.getDishById(this.data.dishId).subscribe({
      next: (dishResponce: FullDishModel) => {
        this.dish = dishResponce
        console.log("получено блюдо: ", dishResponce)
      },
      error: (error) => {
        console.log(error)
      }
    })
  }
}
