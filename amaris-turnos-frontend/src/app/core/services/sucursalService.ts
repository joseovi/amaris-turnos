import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Sucursal } from '../../shared/models/turnoModel';

export interface CrearSucursalRequest {
  nombre: string;
  direccion: string;
  ciudad: string;
}

@Injectable({ providedIn: 'root' })
export class SucursalService {
  private readonly baseUrl = `${environment.apiUrl}/sucursales`;

  constructor(private http: HttpClient) {}

  obtenerTodas(): Observable<Sucursal[]> {
    return this.http.get<Sucursal[]>(this.baseUrl);
  }

  crear(request: CrearSucursalRequest): Observable<Sucursal> {
    return this.http.post<Sucursal>(this.baseUrl, request);
  }

  editar(id: number, request: CrearSucursalRequest): Observable<Sucursal> {
    return this.http.put<Sucursal>(`${this.baseUrl}/${id}`, request);
  }

  cambiarEstado(id: number, activo: boolean): Observable<Sucursal> {
    return this.http.patch<Sucursal>(`${this.baseUrl}/${id}/estado`, { activo });
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}