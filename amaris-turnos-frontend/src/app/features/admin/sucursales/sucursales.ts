import { Component, OnInit, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { SucursalService } from '../../../core/services/sucursalService';
import { Sucursal } from '../../../shared/models/turnoModel';

@Component({
  selector: 'app-sucursales',
  imports: [ReactiveFormsModule],
  templateUrl: './sucursales.html',
  styleUrl: './sucursales.css',
})
export class Sucursales implements OnInit {
  readonly sucursales = signal<Sucursal[]>([]);
  readonly cargando = signal(true);
  readonly guardando = signal(false);
  readonly errorMensaje = signal<string | null>(null);
  readonly mostrarFormulario = signal(false);
  readonly editandoId = signal<number | null>(null);

  readonly form;

  constructor(
    private fb: FormBuilder,
    private sucursalService: SucursalService
  ) {
    this.form = this.fb.group({
      nombre: ['', Validators.required],
      direccion: ['', Validators.required],
      ciudad: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.cargar();
  }

  cargar(): void {
    this.cargando.set(true);
    this.sucursalService.obtenerTodas().subscribe({
      next: (sucursales) => {
        this.sucursales.set(sucursales);
        this.cargando.set(false);
      },
      error: () => {
        this.errorMensaje.set('No se pudieron cargar las sucursales.');
        this.cargando.set(false);
      }
    });
  }

  abrirCrear(): void {
    this.editandoId.set(null);
    this.form.reset();
    this.mostrarFormulario.set(true);
  }

  abrirEditar(sucursal: Sucursal): void {
    this.editandoId.set(sucursal.id);
    this.form.setValue({
      nombre: sucursal.nombre,
      direccion: sucursal.direccion,
      ciudad: sucursal.ciudad
    });
    this.mostrarFormulario.set(true);
  }

  guardar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.guardando.set(true);
    this.errorMensaje.set(null);
    const datos = this.form.getRawValue() as { nombre: string; direccion: string; ciudad: string };
    const id = this.editandoId();

    const peticion = id
      ? this.sucursalService.editar(id, datos)
      : this.sucursalService.crear(datos);

    peticion.subscribe({
      next: () => {
        this.guardando.set(false);
        this.mostrarFormulario.set(false);
        this.cargar();
      },
      error: (err) => {
        this.guardando.set(false);
        this.errorMensaje.set(err.error?.error ?? 'No se pudo guardar la sucursal.');
      }
    });
  }

  alternarEstado(sucursal: Sucursal): void {
    this.sucursalService.cambiarEstado(sucursal.id, !sucursal.activo).subscribe({
      next: () => this.cargar(),
      error: (err) => this.errorMensaje.set(err.error?.error ?? 'No se pudo cambiar el estado.')
    });
  }

  eliminar(sucursal: Sucursal): void {
    const confirmado = confirm(`¿Eliminar permanentemente "${sucursal.nombre}"? Esta acción no se puede deshacer.`);
    if (!confirmado) return;

    this.sucursalService.eliminar(sucursal.id).subscribe({
      next: () => this.cargar(),
      error: (err) => this.errorMensaje.set(err.error?.error ?? 'No se pudo eliminar la sucursal.')
    });
  }
}