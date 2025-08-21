import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faTrash, faEdit } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-food-card',
  standalone: true,
  imports: [CommonModule, FontAwesomeModule],
  templateUrl: './food-card.component.html',
  styleUrls: ['./food-card.component.css']
})
export class FoodCardComponent {
  @Input() id!: string;
  @Input() name!: string;
  @Input() calories!: number;
  @Input() proteins!: number;
  @Input() fats!: number;
  @Input() carbohydrates!: number;
  @Input() backgroundImage?: string;

  @Output() cardClick = new EventEmitter<string>();
  @Output() editClick = new EventEmitter<string>();
  @Output() deleteClick = new EventEmitter<string>();

  faEdit = faEdit;
  faTrash = faTrash;

  onCardClick() {
    this.cardClick.emit(this.id);
  }

  onEditClick(event: Event) {
    event.stopPropagation();
    this.editClick.emit(this.id);
  }

  onDeleteClick(event: Event) {
    event.stopPropagation();
    this.deleteClick.emit(this.id);
  }
}
