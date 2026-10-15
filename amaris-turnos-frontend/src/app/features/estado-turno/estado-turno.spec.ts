import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { EstadoTurno } from './estado-turno';

describe('EstadoTurno', () => {
  let component: EstadoTurno;
  let fixture: ComponentFixture<EstadoTurno>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EstadoTurno],
      providers: [provideRouter([])] 
    }).compileComponents();

    fixture = TestBed.createComponent(EstadoTurno);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});