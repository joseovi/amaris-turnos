import { Component, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/authService';

@Component({
  selector: 'app-registro',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './registro.html',
  styleUrl: './registro.css',
})
export class Registro {
  readonly cargando = signal(false);
  readonly errorMensaje = signal<string | null>(null);

  readonly form;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.form = this.fb.group({
      cedula: ['', [Validators.required, Validators.pattern(/^\d{6,15}$/)]],
      nombre: ['', Validators.required],
      email: ['', Validators.email],
      password: ['', [Validators.required, Validators.minLength(8)]]
    });
  }

  enviar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.cargando.set(true);
    this.errorMensaje.set(null);

    const { cedula, nombre, email, password } = this.form.getRawValue();

    this.authService.registrar({ cedula: cedula!, nombre: nombre!, email: email || undefined, password: password! })
      .subscribe({
        next: () => {
          this.cargando.set(false);
          this.router.navigate([this.authService.rutaInicioSegunRol()]);
        },
        error: (err) => {
          this.cargando.set(false);
          this.errorMensaje.set(err.error?.error ?? 'No se pudo completar el registro.');
        }
      });
  }
}