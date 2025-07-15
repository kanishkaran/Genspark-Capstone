import { inject } from '@angular/core';
import { HttpRequest, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, switchMap } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';
import { Router } from '@angular/router';

export function ErrorInterceptor(req: HttpRequest<unknown>, next: HttpHandlerFn) {
 

  const authService = inject(AuthService);
  const notificationService = inject(NotificationService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      

      if (error.status === 401 ) {
        const refreshToken = authService.getRefreshToken();
        if (refreshToken) {
          return authService.refreshToken().pipe(
            switchMap(() => {
              const token = authService.getAccessToken();
              const cloned = req.clone({
                setHeaders: { Authorization: `Bearer ${token}` }
              });
              return next(cloned);
            }),
            catchError(() => {
              authService.logout();
              router.navigate(['/']);
              return throwError(() => error);
            })
          );
        } else {
          authService.logout();
          router.navigate(['/']);
          return throwError(() => error);
        }
      }

      notificationService.showError('An error occurred');
      return throwError(() => error);
    })
  );
}
