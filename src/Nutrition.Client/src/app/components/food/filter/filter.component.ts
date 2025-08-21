import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { GetFoodRequestParameters } from '../../../models/food-service/Requests/get-food-request-parameters.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-filter',
  standalone: true,
  imports: [CommonModule, FormsModule],
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
        paginatedParameters: {
          page: 1,
          pageSize: 10
        },
        sortingCriteria: null
      };
      this.paramsChange.emit(this.params);
      this.reload.emit();
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

  changePage(offset: number) {
    if (!this.params!.paginatedParameters?.page) {
      this.params!.paginatedParameters!.page = 1;
    }

    this.params!.paginatedParameters!.page += offset;
    this.paramsChange.emit(this.params!);
    this.reload.emit();
  }

  totalPages(): number {
    return Math.ceil(this.totalCount / (this.params?.paginatedParameters?.pageSize || 1));
  }

  hasNextPage(): boolean {
    if (!this.params?.paginatedParameters?.page) return false;
    return this.params.paginatedParameters.page < this.totalPages();
  }

  currentPage(): number {
    return this.params?.paginatedParameters?.page || 1;
  }
}
