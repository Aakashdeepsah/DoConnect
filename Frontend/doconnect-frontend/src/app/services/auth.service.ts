// services/auth.service.ts — Sprint 2
// Changes:
//   1. Stores expiresAt from API response in localStorage
//   2. getSessionTimeLeft() — returns minutes remaining
//   3. isSessionExpiringSoon() — true if < 5 mins left (for warning banner)
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
    return this.http.post<AuthResponse>(`${this.api}/auth/register`, d)
      .pipe(tap(r => this.save(r)));
  }

  login(d: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.api}/auth/login`, d)
      .pipe(tap(r => this.save(r)));
  }

  private save(r: AuthResponse): void {
    localStorage.setItem('token',     r.token);
    localStorage.setItem('username',  r.username);
    localStorage.setItem('role',      r.role);
    localStorage.setItem('userId',    r.userId.toString());
    // Sprint 2: store expiry time
    if (r.expiresAt) {
      localStorage.setItem('expiresAt', new Date(r.expiresAt).getTime().toString());
    }
  }

  logout(): void {
    ['token','username','role','userId','expiresAt'].forEach(k => localStorage.removeItem(k));
  }

  isLoggedIn(): boolean {
    const token = localStorage.getItem('token');
    if (!token) return false;
    // Sprint 2: also check if token is expired
    const expiresAt = localStorage.getItem('expiresAt');
    if (expiresAt && Date.now() > parseInt(expiresAt)) {
      this.logout(); // auto-clear expired session
      return false;
    }
    return true;
  }

  getToken(): string | null    { return localStorage.getItem('token'); }
  getUsername(): string | null { return localStorage.getItem('username'); }
  getRole(): string | null     { return localStorage.getItem('role'); }
  getUserId(): number          { return parseInt(localStorage.getItem('userId') || '0', 10); }
  isAdmin(): boolean           { return this.getRole() === 'Admin'; }

  // Sprint 2: returns minutes left in session (-1 if no expiry stored)
  getSessionTimeLeft(): number {
    const expiresAt = localStorage.getItem('expiresAt');
    if (!expiresAt) return -1;
    const msLeft = parseInt(expiresAt) - Date.now();
    return Math.max(0, Math.floor(msLeft / 60000));
  }

  // Sprint 2: true if session expires in less than 5 minutes
  isSessionExpiringSoon(): boolean {
    const mins = this.getSessionTimeLeft();
    return mins >= 0 && mins < 5;
  }
}
