import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CrearTurnoRequest, Turno } from '../../shared/models/turnoModel';
import { TurnoAdmin } from './turnoAdmin';

@Injectable({ providedIn: 'root' })
export class TurnoService {
  private readonly baseUrl = `${environment.apiUrl}/turnos`;

  constructor(private http: HttpClient) {}

  crearTurno(request: CrearTurnoRequest): Observable<Turno> {
    return this.http.post<Turno>(this.baseUrl, request);
  }

  obtenerPorId(id: number): Observable<Turno> {
    return this.http.get<Turno>(`${this.baseUrl}/${id}`);
  }

  obtenerPorCedula(cedula: string): Observable<Turno[]> {
    return this.http.get<Turno[]>(this.baseUrl, { params: { cedula } });
  }

  activarTurno(id: number): Observable<Turno> {
    return this.http.patch<Turno>(`${this.baseUrl}/${id}/activar`, {});
  }

  obtenerMisTurnosDeHoy(): Observable<Turno[]> {
  return this.http.get<Turno[]>(`${this.baseUrl}/mios`);
  }

  completar(id: number): Observable<TurnoAdmin> {
  return this.http.patch<TurnoAdmin>(`${this.baseUrl}/turnos/${id}/completar`, {});
  }
}