import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { GetFoodRequestParameters } from '../../../models/food-service/Requests/get-food-request-parameters.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PaginationComponent } from '../../pagination/pagination.component';

@Component({
  selector: 'app-filter',
  standalone: true,
  imports: [CommonModule, FormsModule, PaginationComponent],
  templateUrl: './filter.component.html',
  styleUrls: ['./filter.component.css']
})
export class FilterComponent implements OnInit {
  @Input() params: GetFoodRequestParameters | null = null;
  @Input() totalCount!: number;

  @Output() paramsChange = new EventEmitter<GetFoodRequestParameters>();
  @Output() reload = new EventEmitter<void>();

  sortAsc: boolean = false;

  ngOnInit() {
    if (!this.params) {
      this.params = {
        name: null,
        sortAsc: null,
        paginationParameters: {
          page: 1,
          pageSize: 10
        },
        sortingCriteria: null
      };
      this.paramsChange.emit(this.params);
      this.reload.emit()
    }
  }

  onFilterChange() {
    if (this.params!.sortingCriteria != null) {
      this.params!.sortAsc = this.sortAsc;
    } else {
      this.params!.sortAsc = null;
    }
    this.paramsChange.emit(this.params!);
    this.reload.emit();
  }

  changePage() {
    if (this.params!.paginationParameters?.page) {
      this.paramsChange.emit(this.params!);
      this.reload.emit();
    }
  }
}
