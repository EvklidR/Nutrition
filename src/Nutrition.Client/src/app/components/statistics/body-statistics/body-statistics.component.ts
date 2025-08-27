import { Component, OnInit } from '@angular/core'
import { format, sub, addDays } from 'date-fns';
import { curveMonotoneX } from 'd3-shape';
import { NgxChartsModule } from '@swimlane/ngx-charts';

import { ProfileService } from '../../../services/user-service/profile.service';
import { DayResultService } from '../../../services/food-service/day-result.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ShortDayResultResponse } from '../../../models/food-service/Responses/short-day-result.model';
import { ProfileResponse } from '../../../models/user-service/Responses/profile-response.model';
import { PeriodParameters } from '../../../models/request-parameters/period-parameters.model';
import { DateRangePickerComponent } from '../../date-range-picker/date-range-picker.component';

@Component({
  selector: 'app-body-statistics',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    NgxChartsModule,
    DateRangePickerComponent
  ],
  templateUrl: './body-statistics.component.html',
  styleUrl: './body-statistics.component.css'
})
export class BodyStatisticsComponent implements OnInit {
  profile!: ProfileResponse | null;

  dayResults: ShortDayResultResponse[] = [];

  periodParams: PeriodParameters = {
    startDate: sub(new Date(), { weeks: 1 }),
    endDate: new Date()
  }

  weightData: { date: string, weight: number }[] = [];
  imtData: { date: string, imt: number }[] = [];

  chartData: any[] = [];
  colorScheme: string = 'cool';
  curveType = curveMonotoneX;

  yScaleMin = 0;
  yScaleMax: number = 0;

  averageWeight: number = 0;
  weightDifference: number = 0;

  constructor(
    private profileService: ProfileService,
    private dayResultService: DayResultService
  ) { }

  ngOnInit(): void {
    this.profileService.currentProfile$.subscribe((profile) => {

      if (profile) {

        this.profileService.getProfileById(profile!.id).subscribe(
          (fullProfile) => {
            this.profile = fullProfile
            this.loadDayResults();
          }
        )
      }
    });
  }

  loadDayResults(): void {
    this.dayResultService.getDayResults(this.profile!.id, this.periodParams, null).subscribe(
      (response) => {
        this.dayResults = response.dayResults;
        console.log(this.dayResults);
        this.weightData = [];
        this.imtData = [];
        this.generateData();
      }
    );
  }

  generateData(): void {
    for (let currentDate = this.periodParams.startDate; currentDate <= this.periodParams.endDate; currentDate = addDays(currentDate, 1)) {
      const formattedDate = format(currentDate, 'yyyy-MM-dd');

      let day: ShortDayResultResponse | undefined = this.dayResults.find(
        d => d.date === formattedDate
      );
      this.imtData.push({ date: formattedDate, imt: -1 });
      this.weightData.push({ date: formattedDate, weight: day?.weight ?? -1 });
    }

    const hasKnownWeight = this.weightData.some(item => item.weight > -1);

    if (!hasKnownWeight) {
      const currentWeight = this.profile!.weight;
      this.weightData = this.weightData.map(item => ({ ...item, weight: currentWeight }));
    } else {
      let currentValue: number = -1;
      let earliestValue: number = -1;
      for (let i = 0; i < this.weightData.length; i++) {
        if (this.weightData[i].weight > -1) {
          currentValue = this.weightData[i].weight;
          if (earliestValue == -1) {
            earliestValue = this.weightData[i].weight;
          }
        } else {
          this.weightData[i].weight = currentValue;
        }
      }

      for (let i = 0; i < this.weightData.length; i++) {
        if (this.weightData[i].weight == -1) {
          this.weightData[i].weight = earliestValue;
        } else {
          break;
        }
      }
    }

    for (let i = 0; i < this.imtData.length; i++) {
      this.imtData[i].imt = this.weightData[i].weight / (this.profile!.height * this.profile!.height / 10000);
    }

    this.averageWeight = this.weightData.reduce((sum, item) => sum + item.weight, 0) / this.weightData.length;
    const firstWeight = this.weightData[0].weight;
    const lastWeight = this.weightData[this.weightData.length - 1].weight;
    this.weightDifference = lastWeight - firstWeight;
    

    const maxWeight = Math.max(...this.weightData.map(item => item.weight));
    const maxImt = Math.max(...this.imtData.map(item => item.imt));
    this.yScaleMax = Math.max(maxWeight, maxImt) + 10; 

    this.chartData = [
      {
        name: 'Вес (кг)',
        series: this.weightData.map(item => ({
          name: item.date,
          value: item.weight
        }))
      },
      {
        name: 'ИМТ',
        series: this.imtData.map(item => ({
          name: item.date,
          value: item.imt
        }))
      }
    ];

    console.log(this.weightData);
    console.log(this.chartData);
  }
}
