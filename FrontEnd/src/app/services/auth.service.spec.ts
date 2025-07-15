import { TestBed } from '@angular/core/testing';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth.service';
import { RegisterRequest } from '../models/requestModel';
import { provideHttpClient } from '@angular/common/http';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        AuthService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    localStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should log in and store tokens', () => {
    const loginRes = {
      data: {
        token: 'jwt-token',
        refreshToken: 'refresh-token',
        username: 'john',
        role: 'Admin'
      }
    };

    service.login('john', 'password').subscribe(res => {
      expect(res).toEqual(loginRes);
      expect(localStorage.getItem('accessToken')).toBe('jwt-token');
      expect(localStorage.getItem('refreshToken')).toBe('refresh-token');
      expect(localStorage.getItem('currUser')).toBe('john');
      expect(localStorage.getItem('role')).toBe('Admin');
    });

    const req = httpMock.expectOne(`${service['apiUrl']}/Authentication/login`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ username: 'john', password: 'password' });
    req.flush(loginRes);
  });

  it('should register employee', () => {
    const reqData: RegisterRequest = {
      firstname: 'Jane',
      lastname: 'Doe',
      email: 'jane@example.com',
      contactnumber: '1234567890',
      password: 'testpass'
    };

    service.registerEmployee(reqData).subscribe(res => {
      expect(res).toEqual(reqData);
    });

    const req = httpMock.expectOne(`${service['apiUrl']}/Employee`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(reqData);
    req.flush(reqData);
  });

  it('should refresh token and update localStorage', () => {
    localStorage.setItem('refreshToken', 'old-refresh');
    const refreshed = {
      token: 'new-token',
      refreshToken: 'new-refresh-token',
      username: 'john',
      role: 'Admin'
    };

    service.refreshToken().subscribe(() => {
      expect(localStorage.getItem('accessToken')).toBe('new-token');
      expect(localStorage.getItem('refreshToken')).toBe('new-refresh-token');
    });

    const req = httpMock.expectOne(`${service['apiUrl']}/refresh`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ refreshToken: 'old-refresh' });
    req.flush(refreshed);
  });

  it('should logout and clear localStorage', () => {
    localStorage.setItem('accessToken', 'abc');
    localStorage.setItem('refreshToken', 'ref');
    localStorage.setItem('currUser', 'user');
    localStorage.setItem('role', 'Admin');

    service.logout();

    expect(localStorage.getItem('accessToken')).toBeNull();
    expect(localStorage.getItem('refreshToken')).toBeNull();
    expect(localStorage.getItem('currUser')).toBeNull();
    expect(localStorage.getItem('role')).toBeNull();
    expect(service.currUserSubject.value).toBeNull();
  });


  it('should detect authentication', () => {
    expect(service.isAuthenticated()).toBeFalse();
    localStorage.setItem('accessToken', 'token');
    expect(service.isAuthenticated()).toBeTrue();
  });

  it('should detect admin role from currUserSubject', () => {
    service.currUserSubject.next({ username: 'admin', roleName: 'Admin' });
    expect(service.isAdmin()).toBeTrue();

    service.currUserSubject.next({ username: 'bob', roleName: 'User' });
    expect(service.isAdmin()).toBeFalse();
  });
});
