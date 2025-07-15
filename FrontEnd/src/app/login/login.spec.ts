import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Login } from './login';
import { AuthService } from '../services/auth.service';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { Router } from '@angular/router';

describe('Login Component', () => {
  let component: Login;
  let fixture: ComponentFixture<Login>;
  let mockAuthService: jasmine.SpyObj<AuthService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    mockAuthService = jasmine.createSpyObj('AuthService', ['login', 'registerEmployee']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [Login, ReactiveFormsModule, FormsModule],
      providers: [
        { provide: AuthService, useValue: mockAuthService },
        { provide: Router, useValue: mockRouter }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Login);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the Login component', () => {
    expect(component).toBeTruthy();
  });

  it('should not submit login form if invalid', () => {
    component.loginForm.setValue({ email: '', password: '' });
    component.onLoginSubmit();
    expect(component.error).toBe('Please complete all required login fields.');
  });

  it('should submit login form when valid and navigate', () => {
    const mockResponse = { token: 'abc123' };
    mockAuthService.login.and.returnValue(of(mockResponse));

    component.loginForm.setValue({ email: 'user@example.com', password: 'pass123' });
    component.onLoginSubmit();

    expect(mockAuthService.login).toHaveBeenCalledWith('user@example.com', 'pass123');
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/home']);
  });

  it('should handle login error', () => {
    mockAuthService.login.and.returnValue(throwError(() => ({
      error: { errors: { fields: 'Invalid login' } }
    })));

    component.loginForm.setValue({ email: 'wrong@example.com', password: 'wrongpass' });
    component.onLoginSubmit();

    expect(component.error).toBe('Invalid login');
    expect(component.loading).toBeFalse();
  });

  it('should submit signup form when valid', () => {
    const mockSignupData: any = {
      firstname: 'John',
      lastname: 'Doe',
      email: 'john@example.com',
      password: 'StrongPass1!',
      contactNumber: '1234567890'
    };

    mockAuthService.registerEmployee.and.returnValue(of({ message: 'Registered' }));

    component.userForm.setValue(mockSignupData);
    component.onSignupSubmit();

    expect(mockAuthService.registerEmployee).toHaveBeenCalledWith(mockSignupData);
    expect(component.isSignup).toBeFalse();
  });
});
