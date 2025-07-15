import { inject, Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, GuardResult, MaybeAsync, Router, RouterStateSnapshot } from "@angular/router";
import { AuthService } from "../services/auth.service";



@Injectable()
export class AdminGuard implements CanActivate {
  authService = inject(AuthService);
  router = inject(Router)

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (this.authService.isAdmin()) {
      return true
    }

    this.router.navigate(['home', 'dashboard'])
    return false
  }

}