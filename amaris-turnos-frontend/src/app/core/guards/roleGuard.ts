import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/authService';

export const roleGuard: CanActivateFn = (route) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const rolesPermitidos = route.data['rolesPermitidos'] as string[] | undefined;
  const usuario = authService.obtenerUsuarioActual()();

  if (!usuario) {
    router.navigate(['/login']);
    return false;
  }

  if (rolesPermitidos && !rolesPermitidos.includes(usuario.rol)) {
    router.navigate([authService.rutaInicioSegunRol()]);
    return false;
  }

  return true;
};