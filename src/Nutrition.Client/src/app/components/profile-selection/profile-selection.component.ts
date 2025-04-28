import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { ProfileModel } from '../../models/user-service/Models/profile.model';
import { ProfileService } from '../../services/user-service/profile.service';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-profile-selection',
  standalone: true,
  imports: [
    NgFor
  ],
  templateUrl: './profile-selection.component.html',
  styleUrls: ['./profile-selection.component.css']
})
export class ProfileSelectionComponent implements OnInit {
  profiles: ProfileModel[] = [];

  constructor(private router: Router, private profileService: ProfileService) { }

  ngOnInit(): void {
    this.loadProfiles();
  }

  loadProfiles(): void {
    this.profileService.getUserProfiles().subscribe({
      next: (data) => {
        this.profiles = data;
      },
      error: (err) => {
        console.error('Ошибка при загрузке профилей:', err);
      }
    });
  }

  selectProfile(profile: ProfileModel): void {
    this.profileService.setCurrentProfile(profile.id);
    this.profileService.loadCurrentProfile()
    this.router.navigate(['/home']);
  }

  createNewProfile(): void {
    this.router.navigate(['/create-profile']);
  }
}
