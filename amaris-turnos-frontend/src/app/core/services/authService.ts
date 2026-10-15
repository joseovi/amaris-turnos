import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse, RegistroRequest } from '../../shared/models/authModel';

const TOKEN_KEY = 'turnos_banco_token';
const USUARIO_KEY = 'turnos_banco_usuario';

export interface UsuarioActual {
  cedula: string;
  nombre: string;
  rol: 'Usuario' | 'Admin';
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly autenticado = signal<boolean>(this.hayTokenValido());
  private readonly usuarioActual = signal<UsuarioActual | null>(this.leerUsuarioGuardado());

  constructor(private http: HttpClient) {}

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/auth/login`, request)
      .pipe(tap((respuesta) => this.guardarSesion(respuesta)));
  }

  registrar(request: RegistroRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/auth/registro`, request)
      .pipe(tap((respuesta) => this.guardarSesion(respuesta)));
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USUARIO_KEY);
    this.autenticado.set(false);
    this.usuarioActual.set(null);
  }

  obtenerToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  estaAutenticado() {
    return this.autenticado.asReadonly();
  }

  obtenerUsuarioActual() {
    return this.usuarioActual.asReadonly();
  }

  esAdmin(): boolean {
    return this.usuarioActual()?.rol === 'Admin';
  }

  rutaInicioSegunRol(): string {
    return this.esAdmin() ? '/admin' : '/user/agendar';
  }

  private guardarSesion(respuesta: LoginResponse): void {
    localStorage.setItem(TOKEN_KEY, respuesta.token);
    const usuario: UsuarioActual = { cedula: respuesta.cedula, nombre: respuesta.nombre, rol: respuesta.rol };
    localStorage.setItem(USUARIO_KEY, JSON.stringify(usuario));
    this.autenticado.set(true);
    this.usuarioActual.set(usuario);
  }

  private hayTokenValido(): boolean {
    return !!localStorage.getItem(TOKEN_KEY);
  }

  private leerUsuarioGuardado(): UsuarioActual | null {
    const raw = localStorage.getItem(USUARIO_KEY);
    return raw ? JSON.parse(raw) : null;
  }
}