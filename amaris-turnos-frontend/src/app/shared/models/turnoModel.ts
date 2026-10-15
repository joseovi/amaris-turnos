export interface Sucursal {
  id: number;
  nombre: string;
  direccion: string;
  ciudad: string;
  activo: boolean;
}

export type EstadoTurno = 'Pending' | 'Active' | 'Expired' | 'Completed' | 'Cancelled';

export interface Turno {
  id: number;
  cedula: string;
  sucursalId: number;
  sucursalNombre: string;
  estado: EstadoTurno;
  fechaCreacionUtc: string;
  fechaExpiracionUtc: string;
  fechaActivacionUtc: string | null;
  fechaFinalizacionUtc: string | null;
}

export interface CrearTurnoRequest {
  sucursalId: number;
}