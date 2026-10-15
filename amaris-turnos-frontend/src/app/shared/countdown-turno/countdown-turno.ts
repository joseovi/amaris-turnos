import { Component, input, output, OnInit, OnDestroy, signal, computed } from '@angular/core';

@Component({
  selector: 'app-countdown-turno',
  imports: [],
  templateUrl: './countdown-turno.html',
  styleUrl: './countdown-turno.css',
})
export class CountdownTurno implements OnInit, OnDestroy {
  readonly fechaExpiracionUtc = input.required<string>();
  readonly expirado = output<void>();

  readonly segundosRestantes = signal(0);
  readonly porcentajeRestante = signal(100);
  readonly colorBarra = computed(() => {
  const pct = this.porcentajeRestante();
  const hue = pct > 50
    ? 45 + ((pct - 50) / 50) * (142 - 45)  
    : (pct / 50) * 45;                      
  return `hsl(${hue}, 75%, 45%)`;
});

  private intervalId?: ReturnType<typeof setInterval>;
  private readonly totalSegundos = 15 * 60; // 15 minutos, coincide con la regla de negocio del backend

  readonly textoTiempo = computed(() => {
    const s = this.segundosRestantes();
    if (s <= 0) return '00:00';
    const minutos = Math.floor(s / 60);
    const segundos = s % 60;
    return `${minutos.toString().padStart(2, '0')}:${segundos.toString().padStart(2, '0')}`;
  });

  ngOnInit(): void {
    this.actualizar();
    this.intervalId = setInterval(() => this.actualizar(), 1000);
  }

  ngOnDestroy(): void {
    if (this.intervalId) clearInterval(this.intervalId);
  }

  private actualizar(): void {
    const expiraUtc = new Date(this.fechaExpiracionUtc()).getTime();
    const ahoraUtc = Date.now();
    const restanteMs = expiraUtc - ahoraUtc;
    const restanteSeg = Math.max(0, Math.floor(restanteMs / 1000));

    this.segundosRestantes.set(restanteSeg);
    this.porcentajeRestante.set(Math.max(0, Math.min(100, (restanteSeg / this.totalSegundos) * 100)));

    if (restanteSeg <= 0 && this.intervalId) {
      clearInterval(this.intervalId);
      this.expirado.emit();
    }
  }
}