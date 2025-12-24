import { Injectable } from '@angular/core';
//import { Router } from '../../../node_modules/@angular/router/router_module.d';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
  email: string;
  role: string;
}

@Injectable({
  providedIn: 'root',
})

export class AuthService {
  private apiUrl = 'http://localhost:5000/api/Auth';
  constructor(
    private http: HttpClient,
    private router: Router
  ) { }

  login(username: string, password: string): Observable<any> { 
    return this.http.post<any>(`${this.apiUrl}/login`, {
      username,
      password
    }).pipe(
      tap(response => {
        localStorage.setItem('token', response.token); 
      })
    );
  }

  register(userData: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, userData);
  }

  getToken(): string | null {
    return localStorage.getItem('token')
  }

  isLoggedIn(): boolean {
    return this.getToken() !== null;
  }

  logout(): void {
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }

}


export class Auth {

}
