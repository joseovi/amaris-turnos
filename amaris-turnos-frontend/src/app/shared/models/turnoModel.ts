export interface Sucursal {
  id: number;
  nombre: string;
  direccion: string;
  ciudad: string;
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
  cedula: string;
  sucursalId: number;
}