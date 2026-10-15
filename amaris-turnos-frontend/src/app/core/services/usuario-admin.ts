import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface UsuarioAdmin {
  id: number;
  cedula: string;
  nombre: string;
  email: string | null;
  rol: 'Usuario' | 'Admin';
  activo: boolean;
  createdAtUtc: string;
}

export interface CrearUsuarioAdminRequest {
  cedula: string;
  nombre: string;
  email?: string;
  password: string;
  rol: string;
}

export interface EditarUsuarioRequest {
  nombre: string;
  email?: string;
}

@Injectable({ providedIn: 'root' })
export class UsuarioAdminService {
  private readonly baseUrl = `${environment.apiUrl}/admin/usuarios`;

  constructor(private http: HttpClient) {}

  listar(busqueda?: string): Observable<UsuarioAdmin[]> {
    let params = new HttpParams();
    if (busqueda) {
      params = params.set('busqueda', busqueda);
    }
    return this.http.get<UsuarioAdmin[]>(this.baseUrl, { params });
  }

  crear(request: CrearUsuarioAdminRequest): Observable<UsuarioAdmin> {
    return this.http.post<UsuarioAdmin>(this.baseUrl, request);
  }

  editar(id: number, request: EditarUsuarioRequest): Observable<UsuarioAdmin> {
    return this.http.put<UsuarioAdmin>(`${this.baseUrl}/${id}`, request);
  }

  cambiarEstado(id: number, activo: boolean): Observable<UsuarioAdmin> {
    return this.http.patch<UsuarioAdmin>(`${this.baseUrl}/${id}/estado`, { activo });
  }

  cambiarRol(id: number, rol: string): Observable<UsuarioAdmin> {
    return this.http.patch<UsuarioAdmin>(`${this.baseUrl}/${id}/rol`, { rol });
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}