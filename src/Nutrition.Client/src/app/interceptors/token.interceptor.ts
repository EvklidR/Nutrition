import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap } from 'rxjs/operators';
import { throwError } from 'rxjs';

import { UserService } from '../services/user-service/user.service';

export const tokenInterceptor: HttpInterceptorFn = (request, next) => {
  const userService = inject(UserService);

  const accessToken = localStorage.getItem('accessToken');

  if (request.url.includes('/token/refresh')) {
    return next(request);
  }

  if (accessToken) {
    request = request.clone({
      setHeaders: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
  }

  return next(request).pipe(
    catchError((error) => {
      if (error.status === 401) {
        return userService.refreshToken().pipe(
          switchMap((response) => {
            localStorage.setItem('accessToken', response.accessToken);
            localStorage.setItem('refreshToken', response.refreshToken);

            const retryRequest = request.clone({
              setHeaders: {
                Authorization: `Bearer ${response.accessToken}`,
              },
            });

            return next(retryRequest);
          }),
          catchError((err) => {
            userService.logout();
            return throwError(() => err);
          })
        );
      }
      return throwError(() => error);
    })
  );
};
