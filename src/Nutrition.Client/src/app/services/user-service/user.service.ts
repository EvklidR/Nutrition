import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RegisterUserModel } from '../../models/user-service/Requests/register-user.model';
import { LoginUserModel } from '../../models/user-service/Requests/login-user.model';
import { map, tap } from 'rxjs/operators';
import { Observable, BehaviorSubject, EMPTY } from 'rxjs';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';


import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthenticatedResponse } from '../../models/user-service/Responces/authenticated-response.model';
import { RefreshTokenModel } from '../../models/user-service/Requests/refresh-token.model';
import { RevokeTokenModel } from '../../models/user-service/Requests/revoke-token.model';
import { SendConfirmationToEmailModel } from '../../models/user-service/Requests/send-confirmation.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private readonly baseUrl: string = 'https://localhost/user_service/user';

  constructor(private http: HttpClient, private router: Router) { }

  login(command: LoginUserModel): Observable<AuthenticatedResponse> {
    return this.http.post(`${this.baseUrl}/login`, command).pipe(
      map((response: any) => {
        if (response.accessToken && response.refreshToken) {
          this.setAccessToken(response.accessToken);
          this.setRefreshToken(response.refreshToken);
          const role = this.getUserRoleFromToken()
          if (role != null) {
            this.setUserRole(role)
          }
        }
        return response;
      })
    );
  }

  register(command: RegisterUserModel): Observable<void> {
    return this.http.post(`${this.baseUrl}/register`, command).pipe(
      map((response: any) => {
        if (response?.accessToken && response?.refreshToken) {
          this.setAccessToken(response.accessToken);
          this.setRefreshToken(response.refreshToken);
        }
        return response;
      })
    );
  }

  refreshToken(): Observable<AuthenticatedResponse> {

    const command: RefreshTokenModel = {
      accessToken: this.getAccessToken(),
      refreshToken: this.getRefreshToken(),
    }

    if (!command.accessToken || !command.refreshToken) {
      return throwError(() => new HttpErrorResponse({
        status: 401,
        statusText: "Unauthorized",
        error: { message: "Missing tokens" }
      }));
    }

    return this.http.post(`${this.baseUrl}/refresh`, command).pipe(
      tap((response: any) => {
        this.setAccessToken(response.accessToken);
        this.setRefreshToken(response.refreshToken);
      })
    );
  }

  revokeToken(): Observable<void> {
    const refreshToken = this.getRefreshToken();

    if (!refreshToken) {
      return EMPTY;
    }

    const command: RevokeTokenModel = {
      userId: null,
      refreshToken: refreshToken
    };

    return this.http.post<void>(`${this.baseUrl}/revoke`, command);
  }

  sendConfirmation(command: SendConfirmationToEmailModel): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/sendConfirmation`, command);
  }

  logout(): void {
    this.revokeToken()
    this.cleanLocalStorage()
    this.router.navigate(['/login']);
  }

  private getAccessToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  private getRefreshToken(): string | null {
    return localStorage.getItem('refreshToken');
  }

  private setAccessToken(token: string): void {
    localStorage.setItem('accessToken', token);
  }

  private setRefreshToken(token: string): void {
    localStorage.setItem('refreshToken', token);
  }

  private getUserRole(): string | null {
    return localStorage.getItem('role');
  }

  private setUserRole(token: string): void {
    localStorage.setItem('role', token);
  }

  private cleanLocalStorage(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('role');
    localStorage.removeItem('currentProfileId');
  }

  isAuthenticated(): boolean {
    const token = localStorage.getItem('accessToken');
    return !!token;
  }

  isAdmin(): boolean {
    return this.getUserRole() == "admin";
  }

  private getUserRoleFromToken(): string | null {
    const token = localStorage.getItem('accessToken');

    if (!token) {
      this.logout();
      return null;
    }

    const payload = token.split('.')[1];
    if (!payload) {
      this.logout();
      return null;
    }

    try {
      const decodedPayload = atob(payload);
      const payloadObject = JSON.parse(decodedPayload);
      const role = payloadObject['role'] || payloadObject['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || null;

      if (!role) {
        this.logout();
        return null;
      }

      return role;
    }
    catch (error) {
      console.error('Ошибка при декодировании токена:', error);
      this.logout();
      return null;
    }
  }
}
