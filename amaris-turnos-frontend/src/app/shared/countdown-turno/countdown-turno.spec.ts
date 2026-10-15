import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CountdownTurno } from './countdown-turno';

describe('CountdownTurno', () => {
  let component: CountdownTurno;
  let fixture: ComponentFixture<CountdownTurno>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CountdownTurno],
    }).compileComponents();

    fixture = TestBed.createComponent(CountdownTurno);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
