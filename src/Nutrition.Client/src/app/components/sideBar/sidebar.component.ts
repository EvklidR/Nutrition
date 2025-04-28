import { Component, OnInit, ElementRef, Renderer2 } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { NgIf, NgFor, AsyncPipe } from "@angular/common"

import { ProfileModel } from '../../models/user-service/Models/profile.model';
import { UserService } from '../../services/user-service/user.service';
import { ProfileService } from '../../services/user-service/profile.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    RouterModule,
    NgIf,
    NgFor,
    AsyncPipe
  ],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {
  profiles$!: Observable<ProfileModel[]>;
  currentUser$!: Observable<ProfileModel | null>;
  showProfileList: boolean = false;

  constructor(
    private router: Router,
    private userService: UserService,
    private profileService: ProfileService,
    private elRef: ElementRef,
    private renderer: Renderer2
  ) { }

  ngOnInit(): void {
    this.profiles$ = this.profileService.profiles$;
    this.currentUser$ = this.profileService.currentProfile$;

    this.profileService.getUserProfiles().subscribe(() => {
      this.profileService.loadCurrentProfile();
    });

    this.renderer.listen('document', 'click', (event) => {
      this.showProfileList = false;
    });
  }

  toggleProfileList(event: MouseEvent): void {
    event.stopPropagation();
    this.showProfileList = !this.showProfileList;
  }

  selectProfile(profile: ProfileModel, event: MouseEvent): void {
    event.stopPropagation();

    this.profileService.setCurrentProfile(profile.id);
    this.profileService.loadCurrentProfile();

    this.showProfileList = false;
  }

  createProfile(): void {
    this.router.navigate(['/create-profile']);
  }

  logout(): void {
    this.userService.logout();
    this.profileService.clearCurrentProfile();
    this.router.navigate(['/login']);
  }
}
