import { Component, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { TurnoAdminService, TurnoAdmin } from '../../../core/services/turnoAdmin';
import { SucursalService } from '../../../core/services/sucursalService';
import { Sucursal } from '../../../shared/models/turnoModel';

@Component({
  selector: 'app-turnos',
  imports: [DatePipe, ReactiveFormsModule],
  templateUrl: './turnos.html',
  styleUrl: './turnos.css',
})
export class Turnos implements OnInit {
  readonly turnos = signal<TurnoAdmin[]>([]);
  readonly sucursales = signal<Sucursal[]>([]);
  readonly cargando = signal(true);
  readonly errorMensaje = signal<string | null>(null);
  readonly ahora = signal<number>(Date.now());

  private intervalId: any;
  readonly TIEMPO_LIMITE_MINUTOS = 15;

  readonly filtros;

  constructor(
    private fb: FormBuilder,
    private turnoAdminService: TurnoAdminService,
    private sucursalService: SucursalService
  ) {
    this.filtros = this.fb.group({
      fecha: [''],
      sucursalId: [''],
      estado: [''],
      cedula: ['']
    });
  }

  ngOnInit(): void {
    this.sucursalService.obtenerTodas().subscribe((s) => this.sucursales.set(s));
    this.buscar();

    this.intervalId = setInterval(() => {
      this.ahora.set(Date.now());
    },1000);
  }
  
  ngOnDestroy(): void {
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
  }

  buscar(): void {
    this.cargando.set(true);
    const { fecha, sucursalId, estado, cedula } = this.filtros.getRawValue();

    this.turnoAdminService.listar({
      fecha: fecha || undefined,
      sucursalId: sucursalId ? Number(sucursalId) : undefined,
      estado: estado || undefined,
      cedula: cedula || undefined
    }).subscribe({
      next: (turnos) => {
        this.turnos.set(turnos);
        this.cargando.set(false);
      },
      error: () => {
        this.errorMensaje.set('No se pudieron cargar los turnos.');
        this.cargando.set(false);
      }
    });
  }

  cancelar(id: number): void {
    this.turnoAdminService.cancelar(id).subscribe({
      next: () => this.buscar(),
      error: (err) => this.errorMensaje.set(err.error?.error ?? 'No se pudo cancelar el turno.')
    });
  }

  puedeCancelar(turno: TurnoAdmin): boolean {
    return turno.estado === 'Pending' || turno.estado === 'Active';
  }

  completar(id: number): void {
    this.turnoAdminService.completar(id).subscribe({
      next: () => this.buscar(),
      error: (err) => this.errorMensaje.set(err.error?.error ?? 'No se pudo completar el turno.')
    });
  }

  puedeCompletar(turno: TurnoAdmin): boolean {
    return turno.estado === 'Active';
  }

  obtenerTiempoRestante(fechaUtc: string): { texto: string; porcentaje: number } {
    const fechaCreacion = new Date(fechaUtc).getTime();
    const limiteMs = this.TIEMPO_LIMITE_MINUTOS * 60 * 1000;
    const tiempoTranscurrido = this.ahora() - fechaCreacion;
    const restanteMs = limiteMs - tiempoTranscurrido;

    if (restanteMs <= 0) {
      return { texto: '00:00', porcentaje: 0 };
  }

  const minutos = Math.floor(restanteMs / 1000 / 60);
    const segundos = Math.floor((restanteMs / 1000) % 60);

    const mm = minutos.toString().padStart(2, '0');
    const ss = segundos.toString().padStart(2, '0');
    const porcentaje = Math.max(0, (restanteMs / limiteMs) * 100);

    return { texto: `${mm}:${ss}`, porcentaje };
  }
}