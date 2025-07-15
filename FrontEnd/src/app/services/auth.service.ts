import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap, BehaviorSubject } from 'rxjs';
import { LoginRequest, RegisterRequest } from '../models/requestModel';
import { environment } from '../../environment/environment';



@Injectable({
  providedIn: 'root'
})
export class AuthService {
  getToken() {
    throw new Error('Method not implemented.');
  }
  private readonly apiUrl = environment.apiUrl;
  private readonly tokenKey = 'accessToken';
  private readonly refreshKey = 'refreshToken';
  loginReq: LoginRequest = { username: '', password: '' }
  currUserSubject: BehaviorSubject<any | null> = new BehaviorSubject<any | null>(null);
  currUser$ = this.currUserSubject.asObservable()

  constructor(private http: HttpClient) {
    this.loadStoredUser()
  }
  loadStoredUser() {
    var currUser = this.getUser()
    const user = {
      username: currUser.username,
      roleName: currUser.roleName
    }

    this.currUserSubject.next(user);
    console.log(`load user called in auth service ${this.currUserSubject.value.roleName}`)
  }

  login(username: string, password: string): Observable<any> {
    this.loginReq = { username: username.trim(), password: password.trim() }
    return this.http.post<any>(`${this.apiUrl}/Authentication/login`, this.loginReq).pipe(
      tap(res => {
        console.log(res)
        this.storeData(res.data);
        const user = {
          username: res.data.username,
          roleName: res.data.role
        }
        this.currUserSubject.next(user);
        // console.log(this.currUserSubject.value)
      })
    );
  }

  registerEmployee(request: RegisterRequest): Observable<any> {
    return this.http.post<RegisterRequest>(`${this.apiUrl}/Employee`, request)
  }

  refreshToken(): Observable<any> {
    const refreshToken = localStorage.getItem(this.refreshKey);
    return this.http.post<any>(`${this.apiUrl}/refresh`, { refreshToken }).pipe(
      tap(res => this.storeData(res))
    );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.refreshKey);
    localStorage.removeItem('role');
    localStorage.removeItem('currUser');
    this.currUserSubject.next(null);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  getRefreshToken() {
    return localStorage.getItem(this.refreshKey);
  }

  getUser() {
    const user = {
      username: localStorage.getItem('currUser'),
      roleName: localStorage.getItem('role')
    }
    return user
  }

  isAuthenticated(): boolean {
    return !!this.getAccessToken();
  }

  isAdmin(): boolean {
    return this.currUserSubject.value?.roleName === 'Admin';
  }

  private storeData(data: any): void {
    localStorage.setItem(this.tokenKey, data.token);
    localStorage.setItem(this.refreshKey, data.refreshToken);
    localStorage.setItem('currUser', data.username);
    localStorage.setItem('role', data.role);
  }


}