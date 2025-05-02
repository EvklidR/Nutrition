import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { ProfileModel } from '../../models/user-service/Models/profile.model';
import { ActivityLevel } from '../../models/user-service/Enums/activity-level.enum';
import { ProfileService } from '../../services/user-service/profile.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UpdateProfileModel } from '../../models/user-service/Requests/update-profile.model';
import { DayResultService } from '../../services/food-service/day-result.service';
import { UpdateDayResultModel } from '../../models/food-service/Requests/update-day-result.model';
import { Gender } from '../../models/user-service/Enums/gender.enum';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../modals/confirm-dialog-modal/confirm-dialog.component';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-profile-info',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './profile-info.component.html',
  styleUrls: ['./profile-info.component.css']
})
export class ProfileInfoComponent implements OnInit {
  profile!: ProfileModel | null;
  private profileSubscription!: Subscription;

  currentWeight: number = 0;
  currentHeight: number = 0;
  activityLevel: ActivityLevel = ActivityLevel.Low;

  bmi: number = 0;

  activityLevelOptions: { label: string, value: ActivityLevel }[] = [
    { label: 'Седентарный (малоподвижный)', value: ActivityLevel.Sedentary },
    { label: 'Низкий', value: ActivityLevel.Low },
    { label: 'Средний', value: ActivityLevel.Medium },
    { label: 'Высокий', value: ActivityLevel.High },
    { label: 'Очень высокий', value: ActivityLevel.VeryHigh },
  ];

  constructor(
    private router: Router,
    private profileService: ProfileService,
    private dayResultService: DayResultService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.profileSubscription = this.profileService.currentProfile$.subscribe((profile) => {
      this.profile = profile;

      if (profile) {
        this.currentWeight = profile.weight || 0;
        this.currentHeight = profile.height || 0;
        this.activityLevel = profile.activityLevel;
        this.calculateBMI();
      }
    });
  }

  calculateBMI(): void {
    if (this.currentHeight > 0 && this.currentWeight > 0) {
      this.bmi = this.currentWeight / ((this.currentHeight / 100) ** 2);
    }
  }

  updateProfile(): void {
    if (this.profile) {
      const updatedProfile: UpdateProfileModel = {
        id: this.profile.id,
        name: this.profile.name,
        desiredGlassesOfWater: this.profile.desiredGlassesOfWater,
        weight: this.currentWeight,
        height: this.currentHeight,
        activityLevel: Number(this.activityLevel)
      }

      this.profileService.updateProfile(updatedProfile).subscribe((updatedProfile) => {
        console.log("Обновлено");
        this.sendPopUpNotification("Информация успешно обновлена!")
        const dayResult = this.dayResultService.getOrCreateDayResult(this.profile!.id).subscribe({
          next: (currentDayResult) => {
            const updateDayResult: UpdateDayResultModel = {
              id: currentDayResult.id,
              glassesOfWater: currentDayResult.glassesOfWater,
              weight: this.currentWeight
            }

            this.dayResultService.updateDayResult(updateDayResult).subscribe({
              next: () => {
                console.log("обновился результат дня")
              }
            })
          }
        })

        this.calculateBMI();
      });
    }
  }

  getActivityLevelString(value: ActivityLevel): string {
    switch (value) {
      case ActivityLevel.Sedentary: return 'Седентарный (малоподвижный)';
      case ActivityLevel.Low: return 'Низкий';
      case ActivityLevel.Medium: return 'Средний';
      case ActivityLevel.High: return 'Высокий';
      case ActivityLevel.VeryHigh: return 'Очень высокий';
      default: return '';
    }
  }

  getGenderString(value: Gender): string {
    switch (value) {
      case Gender.Male: return 'Мужской';
      case Gender.Female: return 'Женский';
      default: return '';
    }
  }

  deleteProfile(): void {
    this.profileService.deleteProfile(this.profile!.id).subscribe({
      next: () => {
        this.profileService.clearCurrentProfile()
        this.router.navigate(["/profile-selection"])
      }
    })
  }

  openDeleteConfirmation(): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Удаление профиля',
        message: 'Вы уверены, что хотите удалить этот профиль? Все его данные будут стерты'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteProfile();
      }
    });
  }

  sendPopUpNotification(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000
    });
    return
  }
}
