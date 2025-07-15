import { TestBed } from '@angular/core/testing';

import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthGaurd } from './auth-guard';

describe('AuthGaurd', () => {
  let guard: AuthGaurd;
  let mockAuthService: any;
  let mockRouter: any;

  const dummyRoute = {} as ActivatedRouteSnapshot;
  const dummyState = {} as RouterStateSnapshot;

  beforeEach(() => {
    mockAuthService = {
      isAuthenticated: jasmine.createSpy()
    };

    mockRouter = {
      navigate: jasmine.createSpy()
    };

    TestBed.configureTestingModule({
      providers: [
        AuthGaurd,
        { provide: AuthService, useValue: mockAuthService },
        { provide: Router, useValue: mockRouter }
      ]
    });

    guard = TestBed.inject(AuthGaurd);
  });

 
  it('should return true if user is authenticated', () => {
    mockAuthService.isAuthenticated.and.returnValue(true);

    const result = guard.canActivate(dummyRoute, dummyState);

    expect(result).toBeTrue();
    expect(mockRouter.navigate).not.toHaveBeenCalled();
  });

  it('should return false and redirect if user is not authenticated', () => {
    mockAuthService.isAuthenticated.and.returnValue(false);

    const result = guard.canActivate(dummyRoute, dummyState);

    expect(result).toBeFalse();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/']);
  });

 
  it('should call authService.isAuthenticated', () => {
    mockAuthService.isAuthenticated.and.returnValue(true);

    guard.canActivate(dummyRoute, dummyState);

    expect(mockAuthService.isAuthenticated).toHaveBeenCalledTimes(1);
  });
});
