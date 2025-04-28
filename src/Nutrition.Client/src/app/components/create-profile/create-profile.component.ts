import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { NgIf, CommonModule } from "@angular/common"

import { Gender } from '../../models/user-service/Enums/gender.enum';
import { ActivityLevel } from '../../models/user-service/Enums/activity-level.enum';
import { ProfileService } from '../../services/user-service/profile.service';
import { CreateProfileModel } from '../../models/user-service/Requests/create-profile.model';
import { FormsModule } from '@angular/forms';
import { DayResultService } from '../../services/food-service/day-result.service';
import { UpdateDayResultModel } from '../../models/food-service/Requests/update-day-result.model';

@Component({
  selector: 'app-create-profile',
  standalone: true,
  imports: [
    NgIf,
    CommonModule,
    FormsModule
  ],
  templateUrl: './create-profile.component.html',
  styleUrls: ['./create-profile.component.css']
})
export class CreateProfileComponent {
  profileName: string = '';
  weight: number = 0;
  height: number = 0;
  birthday: Date = new Date();
  gender: Gender = Gender.Male;
  activityLevel: ActivityLevel = ActivityLevel.Low;

  genderEnum = Gender;
  activityLevelEnum = ActivityLevel;

  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    private router: Router,
    private profileService: ProfileService,
    private dayResultService: DayResultService
  ) { }

  onSubmit(): void {
    if (!this.profileName.trim()) {
      this.errorMessage = 'Имя профиля не может быть пустым!';
      return;
    }

    const newProfile: CreateProfileModel = {
      userId: null,
      name: this.profileName,
      weight: this.weight,
      height: this.height,
      birthday: this.birthday,
      gender: Number(this.gender),
      activityLevel: Number(this.activityLevel)
    };

    this.profileService.createProfile(newProfile).subscribe({
      next: (profile) => {
        console.log('Создан новый профиль:', profile);

        this.profileService.getUserProfiles().subscribe({
          next: () => {
            this.profileService.setCurrentProfile(profile.id);
            this.profileService.loadCurrentProfile()
          }
        })

        const dayResult = this.dayResultService.getOrCreateDayResult(profile!.id).subscribe({
          next: (currentDayResult) => {

            const updateDayResult: UpdateDayResultModel = {
              id: currentDayResult.id,
              glassesOfWater: currentDayResult.glassesOfWater,
              weight: profile.weight
            }

            this.dayResultService.updateDayResult(updateDayResult).subscribe({
              next: () => {
                console.log("обновился результат дня")
              }
            })
          }
        })

        this.errorMessage = '';

        this.router.navigate(['/home']);
      },
      error: (err) => {
        console.error('Ошибка при создании профиля:', err);
        this.errorMessage = 'Произошла ошибка при создании профиля!';
        this.successMessage = '';
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/profile-selection']);
  }
}
