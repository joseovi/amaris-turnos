import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

import { UsuarioAdminService } from './usuario-admin';

describe('UsuarioAdminService', () => {
  let service: UsuarioAdminService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(UsuarioAdminService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
