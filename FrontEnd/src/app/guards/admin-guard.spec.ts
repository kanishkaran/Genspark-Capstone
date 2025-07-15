import { TestBed } from '@angular/core/testing';

import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AdminGuard } from './admin-guard';

describe('AdminGuard', () => {
  let guard: AdminGuard;
  let mockAuthService: any;
  let mockRouter: any;

  beforeEach(() => {
    mockAuthService = {
      isAdmin: jasmine.createSpy()
    };

    mockRouter = {
      navigate: jasmine.createSpy()
    };

    TestBed.configureTestingModule({
      providers: [
        AdminGuard,
        { provide: AuthService, useValue: mockAuthService },
        { provide: Router, useValue: mockRouter }
      ]
    });

    guard = TestBed.inject(AdminGuard);
  });

  const dummyRoute = {} as ActivatedRouteSnapshot;
  const dummyState = {} as RouterStateSnapshot;

 
  it('should return true if user is admin', () => {
    mockAuthService.isAdmin.and.returnValue(true);

    const result = guard.canActivate(dummyRoute, dummyState);

    expect(result).toBeTrue();
    expect(mockRouter.navigate).not.toHaveBeenCalled();
  });

 
  it('should return false and redirect to dashboard if user is not admin', () => {
    mockAuthService.isAdmin.and.returnValue(false);

    const result = guard.canActivate(dummyRoute, dummyState);

    expect(result).toBeFalse();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['home', 'dashboard']);
  });

  
  it('should call authService.isAdmin once', () => {
    mockAuthService.isAdmin.and.returnValue(true);

    guard.canActivate(dummyRoute, dummyState);

    expect(mockAuthService.isAdmin).toHaveBeenCalledTimes(1);
  });
});
