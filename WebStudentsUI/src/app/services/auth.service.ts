// src/app/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';

export interface UserAccount {
  id: string;
  username: string;
  role: string;
  linkedPersonId?: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = `${environment.apiUrl}/Auth`;
  private localStorageKey = 'user';

  constructor(private http: HttpClient) {}

  login(username: string, password: string): Observable<UserAccount> {
    return this.http.post<UserAccount>(`${this.apiUrl}/login`, { username, password }).pipe(
      tap(user => {
        localStorage.setItem(this.localStorageKey, JSON.stringify(user));
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.localStorageKey);
  }

  isLoggedIn(): boolean {
    return localStorage.getItem(this.localStorageKey) !== null;
  }

  getUser(): UserAccount | null {
    const data = localStorage.getItem(this.localStorageKey);
    return data ? JSON.parse(data) : null;
  }

  getRole(): string | null {
    return this.getUser()?.role ?? null;
  }
}
