import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { describe, it, expect, beforeEach, afterEach } from 'vitest';
import { AuthService } from './authService'; 
import { environment } from '../../../environments/environment';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AuthService, provideHttpClient(), provideHttpClientTesting()]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    localStorage.clear();
  });

  afterEach(() => {
    httpMock.verify(); 
  });

  it('deberia guardar el token en localStorage tras un login exitoso', () => {
    const respuestaSimulada = {
      token: 'token-falso-123',
      expiraUtc: '2026-01-01T00:00:00Z',
      cedula: '1001599660',
      nombre: 'Admin',
      rol: 'Admin'
    };

    service.login({ cedula: '1001599660', password: '123' }).subscribe();

    const req = httpMock.expectOne(`${environment.apiUrl}/auth/login`);
    expect(req.request.method).toBe('POST');
    req.flush(respuestaSimulada);

    expect(localStorage.getItem('turnos_banco_token')).toBe('token-falso-123');
  });

  it('deberia eliminar el token al hacer logout', () => {
    localStorage.setItem('turnos_banco_token', 'token-existente');

    service.logout();

    expect(localStorage.getItem('turnos_banco_token')).toBeNull();
  });

  it('obtenerToken deberia retornar null si no hay token guardado', () => {
    expect(service.obtenerToken()).toBeNull();
  });
});