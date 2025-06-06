import {HttpInterceptorFn, HttpRequest} from "@angular/common/http";
import {AuthService} from "../auth/auth.service";
import {inject} from "@angular/core";
import {catchError, throwError} from "rxjs";

let isAuthenticated = false;
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  const token = authService.authtoken;

  if (!token) {
    return next(req);
  }

  req = req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`
    }
  })

  return next(req)
    .pipe(
      catchError((err) => {
        if (err.status === 401 && authService.isAuth) {
          authService.logout();
        }

        return throwError(err);
      })
    );
}
