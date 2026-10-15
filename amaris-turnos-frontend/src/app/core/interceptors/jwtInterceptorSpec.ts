import { TestBed } from '@angular/core/testing';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { describe, it, expect, beforeEach, afterEach } from 'vitest';
import { jwtInterceptor } from './jwtInterceptor';

describe('jwtInterceptor', () => {
  let httpClient: HttpClient;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([jwtInterceptor])),
        provideHttpClientTesting()
      ]
    });

    httpClient = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
    localStorage.clear();
  });

  afterEach(() => httpMock.verify());

  it('deberia agregar el header Authorization cuando hay token guardado', () => {
    localStorage.setItem('turnos_banco_token', 'mi-token-de-prueba');

    httpClient.get('/api/turnos').subscribe();

    const req = httpMock.expectOne('/api/turnos');
    expect(req.request.headers.get('Authorization')).toBe('Bearer mi-token-de-prueba');
    req.flush({});
  });

  it('no deberia agregar el header Authorization cuando no hay token', () => {
    httpClient.get('/api/turnos').subscribe();

    const req = httpMock.expectOne('/api/turnos');
    expect(req.request.headers.has('Authorization')).toBe(false);
    req.flush({});
  });
});