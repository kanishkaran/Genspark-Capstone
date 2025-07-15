import { inject, Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';


@Injectable()
export class LoginGuard implements CanActivate{
  authService = inject(AuthService);
  router = inject(Router)

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean{
    if(!this.authService.isAuthenticated()){
      return true
    }

    this.router.navigate(['home', 'dashboard'])
    return false
  }
  
}
