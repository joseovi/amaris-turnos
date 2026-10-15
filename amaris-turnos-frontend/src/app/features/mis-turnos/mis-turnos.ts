import { Component, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { TurnoService } from '../../core/services/turnoService';
import { Turno } from '../../shared/models/turnoModel';
import { CountdownTurno } from '../../shared/countdown-turno/countdown-turno';

@Component({
  selector: 'app-mis-turnos',
  imports: [DatePipe, CountdownTurno],
  templateUrl: './mis-turnos.html',
  styleUrl: './mis-turnos.css',
})
export class MisTurnos implements OnInit {
  readonly turnos = signal<Turno[]>([]);
  readonly cargando = signal(true);
  readonly errorMensaje = signal<string | null>(null);
  readonly activandoId = signal<number | null>(null);

  readonly limiteDiario = 5;

  constructor(
    private turnoService: TurnoService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cargar();
  }

  cargar(): void {
    this.cargando.set(true);
    this.turnoService.obtenerMisTurnosDeHoy().subscribe({
      next: (turnos) => {
        this.turnos.set(turnos);
        this.cargando.set(false);
      },
      error: () => {
        this.errorMensaje.set('No se pudieron cargar tus turnos.');
        this.cargando.set(false);
      }
    });
  }

  activar(turnoId: number): void {
    this.activandoId.set(turnoId);
    this.turnoService.activarTurno(turnoId).subscribe({
      next: () => {
        this.activandoId.set(null);
        this.cargar(); // refresca la lista completa con el estado actualizado
      },
      error: (err) => {
        this.activandoId.set(null);
        this.errorMensaje.set(err.error?.error ?? 'No se pudo activar el turno.');
        this.cargar();
      }
    });
  }

  verDetalle(turnoId: number): void {
    this.router.navigate(['/user/turno', turnoId]);
  }

  irASolicitar(): void {
    this.router.navigate(['/user/agendar']);
  }
}