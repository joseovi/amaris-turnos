import { TestBed } from '@angular/core/testing';

import { TurnoAdmin, TurnoAdminService } from './turnoAdmin';

describe('TurnoAdmin', () => {
  let service: TurnoAdmin;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TurnoAdminService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
