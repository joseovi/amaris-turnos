import { Routes } from '@angular/router';
import { authGuard } from './core/guards/authGuard';
import { roleGuard } from './core/guards/roleGuard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },

  {
    path: 'login',
    loadComponent: () => import('./features/login/login').then((m) => m.Login)
  },
  {
    path: 'registro',
    loadComponent: () => import('./features/registro/registro').then((m) => m.Registro)
  },

  {
    path: 'user',
    canActivate: [authGuard, roleGuard],
    data: { rolesPermitidos: ['Usuario'] },
    children: [
      { path: '', redirectTo: 'agendar', pathMatch: 'full' },
      {
        path: 'agendar',
        loadComponent: () =>
          import('./features/solicitud-turno/solicitud-turno').then((m) => m.SolicitudTurno)
      },
      {
        path: 'mis-turnos',
        loadComponent: () =>
          import('./features/mis-turnos/mis-turnos').then((m) => m.MisTurnos)
      },
      {
        path: 'turno/:id',
        loadComponent: () =>
          import('./features/estado-turno/estado-turno').then((m) => m.EstadoTurno)
      }
    ]
  },

  {
    path: 'admin',
    canActivate: [authGuard, roleGuard],
    data: { rolesPermitidos: ['Admin'] },
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./features/admin/dashboard/dashboard').then((m) => m.Dashboard)
      },
      {
        path: 'usuarios',
        loadComponent: () =>
          import('./features/admin/usuarios/usuarios').then((m) => m.Usuarios)
      },
      {
        path: 'turnos',
        // alias "as TurnosAdmin" solo para que el nombre sea descriptivo dentro de este archivo;
        // la clase real en su archivo sigue llamándose "Turnos"
        loadComponent: () =>
          import('./features/admin/turnos/turnos').then((m) => m.Turnos)
      },
      {
        path: 'sucursales',
        loadComponent: () =>
          import('./features/admin/sucursales/sucursales').then((m) => m.Sucursales)
    }
    ]
  },

  { path: '**', redirectTo: 'login' }
];