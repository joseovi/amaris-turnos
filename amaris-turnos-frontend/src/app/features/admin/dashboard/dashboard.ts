import { Component, OnInit, signal } from '@angular/core';
import { TurnoAdminService, DashboardData } from '../../../core/services/turnoAdmin';

@Component({
  selector: 'app-dashboard',
  imports: [],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  readonly datos = signal<DashboardData | null>(null);
  readonly cargando = signal(true);

  constructor(private turnoAdminService: TurnoAdminService) {}

  ngOnInit(): void {
    this.turnoAdminService.obtenerDashboard().subscribe({
      next: (datos) => {
        this.datos.set(datos);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }
}