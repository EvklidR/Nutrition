import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { ProfileService } from '../../../services/user-service/profile.service';
import { MatDialog } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ProfileMealPlanWithDetailsResponse } from '../../../models/meal-plan-service/Responses/profile-meal-plan-with-details.model';
import { ProfilePlanService } from '../../../services/meal-plan-service/profile-plan.service';
import { PaginationParameters } from '../../../models/request-parameters/pagination-parameters.model';
import { ShortProfileResponse } from '../../../models/user-service/Responses/short-profile-response.model';
import { PeriodParameters } from '../../../models/request-parameters/period-parameters.model';
import { subDays } from 'date-fns';
import { ProfileMealPlansResponse } from '../../../models/meal-plan-service/Responses/profile-meal-plans.model';

@Component({
  selector: 'app-meal-plan-historyw',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    FontAwesomeModule,
  ],
  templateUrl: './meal-plan-history.component.html',
  styleUrls: ['./meal-plan-history.component.css']
})
export class MealPlanHistoryComponent implements OnInit {
  profile!: ShortProfileResponse | null;

  profileMealPlans: ProfileMealPlanWithDetailsResponse[] = [];
  totalCount: number = 0;

  paginationParams: PaginationParameters = {
    page: 1,
    pageSize: 20
  }

  periodParams: PeriodParameters = {
    startDate: subDays(new Date(Date.now()), 30),
    endDate: new Date(Date.now())
  }

  hasNextPage: boolean = false;

  isLoading: boolean = true;

  constructor(
    private router: Router,
    private profileService: ProfileService,
    private profileMealPlanService: ProfilePlanService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.profileService.currentProfile$.subscribe(
      (profile) => {
        if (profile) {
          this.profile = profile
          this.loadProfilePlans();
        }
      })

  }

  loadProfilePlans(): void {
    this.profileMealPlanService.getProfilePlanHistory(this.profile!.id, this.paginationParams, this.periodParams).subscribe({
      next: (data: ProfileMealPlansResponse) => {
        this.profileMealPlans = data.profileMealPlans;
        this.totalCount = data.totalCount;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Ошибка загрузки истории планов профиля:', error);
        this.isLoading = false;
      }
    });
  }
}
