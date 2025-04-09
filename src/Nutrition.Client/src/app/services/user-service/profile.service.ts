import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateProfileModel } from '../../models/user-service/Requests/create-profile.model';
import { UpdateProfileModel } from '../../models/user-service/Requests/update-profile.model';
import { Profile } from '../../models/user-service/Models/profile.model';
import { DailyNeedsResponse } from '../../models/user-service/Responces/daily-needs.model';

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  private readonly baseUrl: string = 'https://localhost/user_service/profile';

  constructor(private http: HttpClient) { }

  createProfile(profile: CreateProfileModel): Observable<Profile> {
    return this.http.post<Profile>(`${this.baseUrl}`, profile);
  }

  getUserProfiles(): Observable<Profile[]> {
    return this.http.get<Profile[]>(`${this.baseUrl}/by-user`);
  }

  getProfileById(profileId: string): Observable<Profile> {
    return this.http.get<Profile>(`${this.baseUrl}/by-id/${profileId}`);
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
}
