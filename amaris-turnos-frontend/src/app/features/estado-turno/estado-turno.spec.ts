import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EstadoTurno } from './estado-turno';

describe('EstadoTurno', () => {
  let component: EstadoTurno;
  let fixture: ComponentFixture<EstadoTurno>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EstadoTurno],
    }).compileComponents();

    fixture = TestBed.createComponent(EstadoTurno);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
