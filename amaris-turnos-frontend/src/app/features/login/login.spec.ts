import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { provideRouter, Router } from '@angular/router';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { Login } from './login';
import { environment } from '../../../environments/environment';

describe('Login', () => {
  let fixture: ComponentFixture<Login>;
  let component: Login;
  let httpMock: HttpTestingController;
  let router: Router;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Login],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])]
    }).compileComponents();

    fixture = TestBed.createComponent(Login);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
    router = TestBed.inject(Router);
  });

  it('el formulario deberia ser invalido al inicio (campos vacios)', () => {
    expect(component.form.valid).toBe(false);
  });

  it('el formulario deberia ser valido con usuario y contraseña completos', () => {
    component.form.setValue({ cedula: '1001599660', password: '123456' });
    expect(component.form.valid).toBe(true);
  });

  it('deberia navegar a /user/agendar tras un login exitoso', () => {
    const navigateSpy = vi.spyOn(router, 'navigate');

    component.form.setValue({ cedula: '1001599660', password: '123456' });
    component.enviar();

    const req = httpMock.expectOne(`${environment.apiUrl}/auth/login`);
    req.flush({ token: 'abc', expiraUtc: '2026-01-01', nombreUsuario: 'admin', rol: 'Admin' });

    expect(navigateSpy).toHaveBeenCalledWith(['/user/agendar']);
  });

  it('deberia mostrar mensaje de error especifico en credenciales incorrectas (401)', () => {
    component.form.setValue({ cedula: '1001599660', password: 'incorrecta' });
    component.enviar();

    const req = httpMock.expectOne(`${environment.apiUrl}/auth/login`);
    req.flush({ error: 'Usuario o contraseña incorrectos.' }, { status: 401, statusText: 'Unauthorized' });

    expect(component.errorMensaje()).toBe('Usuario o contraseña incorrectos.');
  });
});