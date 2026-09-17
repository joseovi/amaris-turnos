export interface LoginRequest {
  nombreUsuario: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiraUtc: string;
  nombreUsuario: string;
  rol: string;
}