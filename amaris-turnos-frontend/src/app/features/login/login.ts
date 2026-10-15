import { Component, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/authService';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
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
      password: ['', Validators.required]
    });
  }

  enviar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.cargando.set(true);
    this.errorMensaje.set(null);

    this.authService.login(this.form.getRawValue() as { cedula: string; password: string })
      .subscribe({
        next: () => {
          this.cargando.set(false);
          this.router.navigate([this.authService.rutaInicioSegunRol()]);
        },
        error: (err) => {
          this.cargando.set(false);
          this.errorMensaje.set(
            err.status === 401
              ? 'Cédula o contraseña incorrectos.'
              : (err.error?.error ?? 'No se pudo conectar con el servidor.')
          );
        }
      });
  }
}