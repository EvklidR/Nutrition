import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';

import { CreateProfileModel } from '../../models/user-service/Requests/create-profile.model';
import { UpdateProfileModel } from '../../models/user-service/Requests/update-profile.model';
import { ProfileModel } from '../../models/user-service/Models/profile.model';
import { DailyNeedsResponse } from '../../models/user-service/Responces/daily-needs.model';

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  private readonly baseUrl: string = 'https://localhost/user_service/profile';

  private currentProfileSubject = new BehaviorSubject<ProfileModel | null>(null);
  public currentProfile$ = this.currentProfileSubject.asObservable();

  private profilesSubject = new BehaviorSubject<ProfileModel[]>([]);
  public profiles$ = this.profilesSubject.asObservable();

  constructor(private http: HttpClient) { }

  createProfile(profile: CreateProfileModel): Observable<ProfileModel> {
    return this.http.post<ProfileModel>(`${this.baseUrl}`, profile);
  }

  getUserProfiles(): Observable<ProfileModel[]> {
    return this.http.get<ProfileModel[]>(`${this.baseUrl}/by-user`).pipe(
      tap((profiles) => {
        this.profilesSubject.next(profiles);
      })
    );
  }

  getProfileById(profileId: string): Observable<ProfileModel> {
    return this.http.get<ProfileModel>(`${this.baseUrl}/by-id/${profileId}`);
  }

  calculateDailyNeeds(profileId: string): Observable<DailyNeedsResponse> {
    return this.http.get<DailyNeedsResponse>(`${this.baseUrl}/${profileId}/daily-needs`);
  }

  updateProfile(profile: UpdateProfileModel): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}`, profile);
  }

  deleteProfile(profileId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${profileId}`);
  }

  setCurrentProfile(profileId: string): void {
    localStorage.setItem('currentProfileId', profileId);
  }

  clearCurrentProfile(): void {
    localStorage.removeItem('currentProfileId');
    this.currentProfileSubject.next(null);
  }

  loadCurrentProfile(): void {
    const profileId = localStorage.getItem('currentProfileId');
    if (profileId) {
      const profile = this.profilesSubject.getValue().find(p => p.id == profileId);
      if (profile) {
        console.log('Found profile, setting current:', profile);
        this.currentProfileSubject.next(profile);
      } else {
        this.currentProfileSubject.next(null);
      }
    } else {
      this.currentProfileSubject.next(null);
    }
  }
}
