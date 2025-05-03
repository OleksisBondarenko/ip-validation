import { inject } from "@angular/core";
import {CanActivateFn, Router} from "@angular/router";
import { AuthService } from "../auth/auth.service";

export const canActiveAuth: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuth) {
    return true;
  }

  return router.navigateByUrl("/login");
}
