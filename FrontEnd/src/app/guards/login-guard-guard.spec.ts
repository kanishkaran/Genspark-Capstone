import { TestBed } from '@angular/core/testing';
import { LoginGuard } from './login-guard-guard';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';


describe('LoginGuard', () => {
  let guard: LoginGuard;
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
        LoginGuard,
        { provide: AuthService, useValue: mockAuthService },
        { provide: Router, useValue: mockRouter }
      ]
    });

    guard = TestBed.inject(LoginGuard);
  })


  it('should return false and redirect if user is authenticated', () => {
    mockAuthService.isAuthenticated.and.returnValue(true);

    const result = guard.canActivate(dummyRoute, dummyState);

    expect(result).toBeFalse();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['home','dashboard']);
  });

});