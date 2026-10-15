export interface LoginRequest {
  cedula: string;
  password: string;
}

export interface RegistroRequest {
  cedula: string;
  nombre: string;
  email?: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiraUtc: string;
  cedula: string;
  nombre: string;
  rol: 'Usuario' | 'Admin';
}