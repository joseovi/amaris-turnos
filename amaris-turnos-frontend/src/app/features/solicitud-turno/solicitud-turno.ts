import { Component, inject, OnInit, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { SucursalService } from '../../core/services/sucursalService';
import { TurnoService } from '../../core/services/turnoService';
import { Sucursal } from '../../shared/models/turnoModel';

@Component({
  selector: 'app-solicitud-turno',
  imports: [ReactiveFormsModule],
  templateUrl: './solicitud-turno.html',
  styleUrl: './solicitud-turno.css',
})
export class SolicitudTurno implements OnInit {
  private readonly fb = inject(FormBuilder);

  readonly sucursales = signal<Sucursal[]>([]);
  readonly cargandoSucursales = signal(true);
  readonly enviando = signal(false);
  readonly errorMensaje = signal<string | null>(null);

  readonly form = this.fb.group({
    cedula: ['', [Validators.required, Validators.pattern(/^\d{6,15}$/)]],
    sucursalId: [null as number | null, Validators.required]
  });

  constructor(
    private sucursalService: SucursalService,
    private turnoService: TurnoService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.sucursalService.obtenerTodas().subscribe({
      next: (sucursales) => {
        this.sucursales.set(sucursales);
        this.cargandoSucursales.set(false);
      },
      error: () => {
        this.errorMensaje.set('No se pudieron cargar las sucursales.');
        this.cargandoSucursales.set(false);
      }
    });
  }

  solicitarTurno(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.enviando.set(true);
    this.errorMensaje.set(null);

    const { cedula, sucursalId } = this.form.getRawValue();

    this.turnoService.crearTurno({ cedula: cedula!, sucursalId: sucursalId! }).subscribe({
      next: (turno) => {
        this.enviando.set(false);
        this.router.navigate(['/turno', turno.id]);
      },
      error: (err) => {
        this.enviando.set(false);
        this.errorMensaje.set(
          err.error?.error ?? 'No se pudo crear el turno. Intenta nuevamente.'
        );
      }
    });
  }
}