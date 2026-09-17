import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/authService';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.obtenerToken();

  if (token) {
    const reqClonada = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
    return next(reqClonada);
  }

  return next(req);
};