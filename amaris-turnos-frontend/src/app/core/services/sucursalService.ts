import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Sucursal } from '../../shared/models/turnoModel';

@Injectable({ providedIn: 'root' })
export class SucursalService {
  private readonly baseUrl = `${environment.apiUrl}/sucursales`;

  constructor(private http: HttpClient) {}

  obtenerTodas(): Observable<Sucursal[]> {
    return this.http.get<Sucursal[]>(this.baseUrl);
  }
}