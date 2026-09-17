import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { TurnoService } from '../../core/services/turnoService';
import { Turno } from '../../shared/models/turnoModel';

@Component({
  selector: 'app-estado-turno',
  imports: [DatePipe],
  templateUrl: './estado-turno.html',
  styleUrl: './estado-turno.css',
})
export class EstadoTurno implements OnInit {
  readonly turno = signal<Turno | null>(null);
  readonly cargando = signal(true);
  readonly errorMensaje = signal<string | null>(null);
  readonly activando = signal(false);

  private turnoId!: number;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private turnoService: TurnoService
  ) {}

  ngOnInit(): void {
    this.turnoId = Number(this.route.snapshot.paramMap.get('id'));
    this.cargarTurno();
  }

  cargarTurno(): void {
    this.cargando.set(true);
    this.turnoService.obtenerPorId(this.turnoId).subscribe({
      next: (turno) => {
        this.turno.set(turno);
        this.cargando.set(false);
      },
      error: () => {
        this.errorMensaje.set('No se pudo cargar el turno.');
        this.cargando.set(false);
      }
    });
  }

  activar(): void {
    this.activando.set(true);
    this.errorMensaje.set(null);

    this.turnoService.activarTurno(this.turnoId).subscribe({
      next: (turno) => {
        this.turno.set(turno);
        this.activando.set(false);
      },
      error: (err) => {
        this.activando.set(false);
        this.errorMensaje.set(err.error?.error ?? 'No se pudo activar el turno.');
        this.cargarTurno();
      }
    });
  }

  solicitarOtroTurno(): void {
    this.router.navigate(['/solicitar-turno']);
  }
}