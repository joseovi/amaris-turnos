import { Component, OnInit, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { UsuarioAdmin, UsuarioAdminService } from '../../../core/services/usuario-admin';

@Component({
  selector: 'app-usuarios',
  imports: [ReactiveFormsModule, DatePipe],
  templateUrl: './usuarios.html',
  styleUrl: './usuarios.css',
})
export class Usuarios implements OnInit {
  readonly usuarios = signal<UsuarioAdmin[]>([]);
  readonly cargando = signal(true);
  readonly errorMensaje = signal<string | null>(null);
  readonly mostrarFormulario = signal(false);
  readonly busqueda = signal('');
  readonly creando = signal(false);
  readonly form;

  constructor(
    private fb: FormBuilder,
    private usuarioAdminService: UsuarioAdminService
  ) {
    this.form = this.fb.group({
      cedula: ['', [Validators.required, Validators.pattern(/^\d{6,15}$/)]],
      nombre: ['', Validators.required],
      email: ['', Validators.email],
      password: ['', [Validators.required, Validators.minLength(8)]],
      rol: ['Usuario', Validators.required]
    });
  }

  ngOnInit(): void {
    this.cargar();
  }

  cargar(): void {
    this.cargando.set(true);
    this.usuarioAdminService.listar(this.busqueda() || undefined).subscribe({
      next: (usuarios) => {
        this.usuarios.set(usuarios);
        this.cargando.set(false);
      },
      error: () => {
        this.errorMensaje.set('No se pudieron cargar los usuarios.');
        this.cargando.set(false);
      }
    });
  }

  buscar(valor: string): void {
    this.busqueda.set(valor);
    this.cargar();
  }

  crear(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.creando.set(true);
    this.errorMensaje.set(null);

    this.usuarioAdminService.crear(this.form.getRawValue() as any).subscribe({
      next: () => {
        this.creando.set(false);
        this.mostrarFormulario.set(false);
        this.form.reset({ rol: 'Usuario' });
        this.cargar();
      },
      error: (err) => {
        this.creando.set(false);
        this.errorMensaje.set(err.error?.error ?? 'No se pudo crear el usuario.');
      }
    });
  }

  alternarEstado(usuario: UsuarioAdmin): void {
    this.usuarioAdminService.cambiarEstado(usuario.id, !usuario.activo).subscribe({
      next: () => this.cargar(),
      error: (err) => this.errorMensaje.set(err.error?.error ?? 'No se pudo cambiar el estado.')
    });
  }

  cambiarRol(usuario: UsuarioAdmin, nuevoRol: string): void {
    this.usuarioAdminService.cambiarRol(usuario.id, nuevoRol).subscribe({
      next: () => this.cargar(),
      error: (err) => this.errorMensaje.set(err.error?.error ?? 'No se pudo cambiar el rol.')
    });
  }

  eliminar(usuario: UsuarioAdmin): void {
      const confirmado = confirm(`¿Eliminar permanentemente a ${usuario.nombre}? Esta acción no se puede deshacer.`);
      if (!confirmado) return;

    this.usuarioAdminService.eliminar(usuario.id).subscribe({
      next: () => this.cargar(),
      error: (err) => this.errorMensaje.set(err.error?.error ?? 'No se pudo eliminar el usuario.')
    });
  }
}