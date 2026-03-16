import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private api = environment.apiUrl;
  constructor(private http: HttpClient) {}

  register(d: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.api}/auth/register`, d).pipe(tap(r => this.save(r)));
  }
  login(d: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.api}/auth/login`, d).pipe(tap(r => this.save(r)));
  }
  private save(r: AuthResponse): void {
    localStorage.setItem('token', r.token); localStorage.setItem('username', r.username);
    localStorage.setItem('role', r.role);   localStorage.setItem('userId', r.userId.toString());
  }
  logout(): void { ['token','username','role','userId'].forEach(k => localStorage.removeItem(k)); }
  isLoggedIn(): boolean        { return !!localStorage.getItem('token'); }
  getToken(): string | null    { return localStorage.getItem('token'); }
  getUsername(): string | null { return localStorage.getItem('username'); }
  getRole(): string | null     { return localStorage.getItem('role'); }
  getUserId(): number          { return parseInt(localStorage.getItem('userId') || '0', 10); }
  isAdmin(): boolean           { return this.getRole() === 'Admin'; }
}
