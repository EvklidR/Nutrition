import { Component, OnInit } from '@angular/core'
import { Router } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import { parseISO, eachDayOfInterval, format, sub, addDays } from 'date-fns';
import { curveLinear, curveCardinal, curveMonotoneX, curveMonotoneY } from 'd3-shape';
import { NgxChartsModule } from '@swimlane/ngx-charts';

import { ProfileModel } from '../../models/user-service/Models/profile.model';
import { DayResultModel } from '../../models/food-service/Responces/day-result.model';
import { UserService } from '../../services/user-service/user.service';
import { ProfileService } from '../../services/user-service/profile.service';
import { DayResultService } from '../../services/food-service/day-result.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-statistics',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    NgxChartsModule
  ],
  templateUrl: './statistics.component.html',
  styleUrl: './statistics.component.css'
})
export class StatisticsComponent implements OnInit {
  profile!: ProfileModel | null;
  private profileSubscription!: Subscription;

  dayResults: DayResultModel[] = [];

  weeksAgo: number = 1;
  availablePeriods: number[] = [1, 2, 3, 4, 8, 12, 18, 24];


  start: Date = sub(new Date(), { weeks: this.weeksAgo });
  end: Date = new Date();

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
    private router: Router,
    private userService: UserService,
    private profileService: ProfileService,
    private dayResultService: DayResultService
  ) { }

  ngOnInit(): void {
    this.profileSubscription = this.profileService.currentProfile$.subscribe((profile) => {
      this.profile = profile;

      if (profile) {
        this.loadDayResults(profile.id);
      }
    });
  }

  get startDate(): string {
    return format(this.start, 'yyyy-MM-dd');
  }

  get endDate(): string {
    return format(this.end, 'yyyy-MM-dd');
  }

  loadDayResults(profileId: string): void {
    this.dayResultService.getDayResultsByPeriod(profileId, this.startDate, this.endDate).subscribe(
      (days) => {
        this.dayResults = days;
        console.log(this.dayResults);
        this.weightData = [];
        this.imtData = [];
        this.generateData();
      }
    );
  }

  generateData(): void {
    for (let currentDate = addDays(this.start, 1); currentDate <= this.end; currentDate = addDays(currentDate, 1)) {
      const formattedDate = format(currentDate, 'yyyy-MM-dd');

      this.dayResults = this.dayResults.map(d => ({
        ...d,
        date: new Date(d.date)
      }));

      let day: DayResultModel | undefined = this.dayResults.find(
        d => format(d.date, 'yyyy-MM-dd') === formattedDate
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

    const knownWeights = this.weightData.filter(item => item.weight > -1);
    if (knownWeights.length > 0) {
      this.averageWeight = knownWeights.reduce((sum, item) => sum + item.weight, 0) / knownWeights.length;
      const firstWeight = knownWeights[0].weight;
      const lastWeight = knownWeights[knownWeights.length - 1].weight;
      this.weightDifference = lastWeight - firstWeight;
    }

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

  navigateToFoodStatistics(): void {
    console.log("here")
    this.router.navigate(["/food-statistics"])
  }

  updatePeriod(): void {
    if (this.profile) {
      this.start = sub(this.end, { weeks: this.weeksAgo });
      this.loadDayResults(this.profile.id);
    }
  }
}
