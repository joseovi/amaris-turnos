import { Component, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/authService';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
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
      nombreUsuario: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  enviar(): void {
    console.log('enviar() se ejecutó', this.form.value, this.form.valid);
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.cargando.set(true);
    this.errorMensaje.set(null);

    this.authService.login(this.form.getRawValue() as { nombreUsuario: string; password: string })
      .subscribe({
        next: () => {
          this.cargando.set(false);
          this.router.navigate(['/solicitar-turno']);
        },
        error: (err) => {
          this.cargando.set(false);
          this.errorMensaje.set(
            err.status === 401
              ? 'Usuario o contraseña incorrectos.'
              : 'No se pudo conectar con el servidor. Intenta nuevamente.'
          );
        }
      });
  }
}