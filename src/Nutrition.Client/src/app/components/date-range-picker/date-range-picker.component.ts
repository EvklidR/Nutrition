import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PeriodParameters } from '../../models/request-parameters/period-parameters.model';
import sub from 'date-fns/esm/sub/index.js';

export interface DateRange {
  startDate: Date;
  endDate: Date;
}

@Component({
  selector: 'app-date-range-picker',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './date-range-picker.component.html',
  styleUrls: ['./date-range-picker.component.css']
})
export class DateRangePickerComponent implements OnInit {
  @Input() params: PeriodParameters | null = null;

  @Output() reload = new EventEmitter<void>();
  @Output() paramsChange = new EventEmitter<PeriodParameters>();

  ngOnInit() {
    if (!this.params) {
      this.params = {
        startDate: sub(new Date(), { weeks: 1 }),
        endDate: new Date()
      };
      this.paramsChange.emit(this.params)
      this.reload.emit()
    }
  }

  onDateChange() {
    this.paramsChange.emit(this.params!);
    this.reload.emit();
  }
}
