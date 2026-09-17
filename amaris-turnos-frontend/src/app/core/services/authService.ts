import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse } from '../../shared/models/authModel';

const TOKEN_KEY = 'amaris_turnos_token';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly autenticado = signal<boolean>(this.hayTokenValido());

  constructor(private http: HttpClient) {}

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/auth/login`, request)
      .pipe(
        tap((respuesta) => {
          localStorage.setItem(TOKEN_KEY, respuesta.token);
          this.autenticado.set(true);
        })
      );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    this.autenticado.set(false);
  }

  obtenerToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  estaAutenticado() {
    return this.autenticado.asReadonly();
  }

  private hayTokenValido(): boolean {
    return !!localStorage.getItem(TOKEN_KEY);
  }
}