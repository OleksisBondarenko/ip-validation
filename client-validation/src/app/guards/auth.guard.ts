import { inject } from "@angular/core";
import {CanActivateFn, Router} from "@angular/router";
import { AuthService } from "../auth/auth.service";
import {UserService} from "../services/user.service";

export const canActiveAuth: CanActivateFn = () => {
  const authService = inject(AuthService);
  const userService = inject(UserService);
  const router = inject(Router);

  if (authService.isAuth()) {
    if (!userService.user())
    {
      userService
        .fetchUserDetailed()
        .subscribe();
    }

    return true;
  }

  return router.navigateByUrl("/login");
}
