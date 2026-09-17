import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SolicitudTurno } from './solicitud-turno';

describe('SolicitudTurno', () => {
  let component: SolicitudTurno;
  let fixture: ComponentFixture<SolicitudTurno>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SolicitudTurno],
    }).compileComponents();

    fixture = TestBed.createComponent(SolicitudTurno);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
