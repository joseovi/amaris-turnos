import { Routes } from '@angular/router';
import { authGuard } from './core/guards/authGuard';

export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/login/login').then((m) => m.Login)
  },
  {
    path: 'solicitar-turno',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/solicitud-turno/solicitud-turno').then(
        (m) => m.SolicitudTurno
      )
  },
  {
    path: 'turno/:id',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/estado-turno/estado-turno').then(
        (m) => m.EstadoTurno
      )
  },
  { path: '**', redirectTo: 'login' }
];
