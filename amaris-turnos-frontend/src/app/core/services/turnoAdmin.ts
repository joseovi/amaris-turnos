import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface TurnoAdmin {
  id: number;
  usuarioCedula: string;
  usuarioNombre: string;
  sucursalNombre: string;
  estado: string;
  fechaCreacionUtc: string;
  fechaExpiracionUtc: string;
  fechaActivacionUtc: string | null;
  fechaFinalizacionUtc: string | null;
}

export interface FiltroTurnosAdmin {
  fecha?: string;
  sucursalId?: number;
  estado?: string;
  cedula?: string;
}

export interface DashboardData {
  turnosHoy: number;
  pendientes: number;
  activos: number;
  completados: number;
  expirados: number;
  cancelados: number;
  totalUsuarios: number;
  totalSucursales: number;
}

@Injectable({ providedIn: 'root' })
export class TurnoAdminService {
  private readonly baseUrl = `${environment.apiUrl}/admin`;

  constructor(private http: HttpClient) {}

  listar(filtro: FiltroTurnosAdmin): Observable<TurnoAdmin[]> {
    const params: Record<string, string> = {};
    if (filtro.fecha) params['fecha'] = filtro.fecha;
    if (filtro.sucursalId) params['sucursalId'] = String(filtro.sucursalId);
    if (filtro.estado) params['estado'] = filtro.estado;
    if (filtro.cedula) params['cedula'] = filtro.cedula;

    return this.http.get<TurnoAdmin[]>(`${this.baseUrl}/turnos`, { params });
  }

  cancelar(id: number): Observable<TurnoAdmin> {
    return this.http.patch<TurnoAdmin>(`${this.baseUrl}/turnos/${id}/cancelar`, {});
  }

  obtenerDashboard(): Observable<DashboardData> {
    return this.http.get<DashboardData>(`${this.baseUrl}/dashboard`);
  }

  completar(id: number): Observable<TurnoAdmin> {
    return this.http.patch<TurnoAdmin>(`${this.baseUrl}/turnos/${id}/completar`, {});
  }
}