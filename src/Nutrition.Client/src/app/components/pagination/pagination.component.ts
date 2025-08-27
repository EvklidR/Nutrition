import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { PaginationParameters } from '../../models/request-parameters/pagination-parameters.model';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.css']
})
export class PaginationComponent implements OnInit {
  @Input() params: PaginationParameters | null = null;
  @Input() totalCount!: number;

  @Output() reload = new EventEmitter<void>();
  @Output() paramsChange = new EventEmitter<PaginationParameters>();

  sortAsc: boolean = false;

  ngOnInit() {
    if (!this.params) {
      this.params = {
        page: 1,
        pageSize: 10
      };
      this.paramsChange.emit(this.params)
      this.reload.emit()
    }
  }

  changePage(offset: number) {
    if (!this.params!.page) {
      this.params!.page = 1;
    }

    this.params!.page += offset;
    this.paramsChange.emit(this.params!);
    this.reload.emit();
  }

  totalPages(): number {
    return Math.ceil(this.totalCount / (this.params?.pageSize || 1));
  }

  hasNextPage(): boolean {
    if (!this.params?.page) return false;
    return this.params.page < this.totalPages();
  }

  currentPage(): number {
    return this.params?.page || 1;
  }
}
