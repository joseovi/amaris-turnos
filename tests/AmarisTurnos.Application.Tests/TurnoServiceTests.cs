using AmarisTurnos.Application.DTOs;
using AmarisTurnos.Application.Interfaces;
using AmarisTurnos.Application.Services;
using AmarisTurnos.Domain.Entities;
using AmarisTurnos.Domain.Exceptions;
using Moq;
using Xunit;

namespace AmarisTurnos.Application.Tests
{
    public class TurnoServiceTests
    {
        private readonly Mock<ITurnoRepository> _turnoRepositoryMock;
        private readonly Mock<ISucursalRepository> _sucursalRepositoryMock;
        private readonly TurnoService _sut; // "sut" = System Under Test

        public TurnoServiceTests()
        {
            _turnoRepositoryMock = new Mock<ITurnoRepository>();
            _sucursalRepositoryMock = new Mock<ISucursalRepository>();
            _sut = new TurnoService(_turnoRepositoryMock.Object, _sucursalRepositoryMock.Object);
        }

        [Fact]
        public async Task CrearTurnoAsync_ConDatosValidos_DeberiaCrearElTurno()
        {
            // Arrange
            int usuarioId = 1;
            var dto = new CrearTurnoDto { SucursalId = 1 };
            var sucursal = new Sucursal { Id = 1, Nombre = "Sucursal Centro" };

            _sucursalRepositoryMock
                .Setup(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sucursal);

            _turnoRepositoryMock
                .Setup(r => r.CrearConValidacionDeLimiteAsync(
                    It.IsAny<Turno>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                    5, It.IsAny<CancellationToken>()))
                .Callback<Turno, DateTime, DateTime, int, CancellationToken>((t, _, _, _, _) =>
                {
                    t.Usuario = new Usuario { Id = usuarioId, Cedula = "123456789" };
                })
                .ReturnsAsync((true, 1));

            // Act -> Pasamos usuarioId (1) como primer parámetro
            var resultado = await _sut.CrearTurnoAsync(usuarioId, dto);

            // Assert
            Assert.Equal("123456789", resultado.Cedula);
            Assert.Equal("Sucursal Centro", resultado.SucursalNombre);
            Assert.Equal("Pending", resultado.Estado);
        }

        [Fact]
        public async Task CrearTurnoAsync_ConSucursalInexistente_DeberiaLanzarExcepcion()
        {
            // Arrange
            int usuarioId = 1;
            var dto = new CrearTurnoDto { SucursalId = 999 };

            _sucursalRepositoryMock
                .Setup(r => r.ObtenerPorIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Sucursal?)null);

            // Act & Assert -> Pasamos usuarioId
            var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(
                () => _sut.CrearTurnoAsync(usuarioId, dto));

            Assert.Contains("sucursal", ex.Message, StringComparison.OrdinalIgnoreCase);

            _turnoRepositoryMock.Verify(
                r => r.CrearConValidacionDeLimiteAsync(
                    It.IsAny<Turno>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                    It.IsAny<int>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task CrearTurnoAsync_CuandoSuperaLimiteDiario_DeberiaLanzarExcepcion()
        {
            // Arrange
            int usuarioId = 1;
            var dto = new CrearTurnoDto { SucursalId = 1 };
            var sucursal = new Sucursal { Id = 1, Nombre = "Sucursal Centro" };

            _sucursalRepositoryMock
                .Setup(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sucursal);

            _turnoRepositoryMock
                .Setup(r => r.CrearConValidacionDeLimiteAsync(
                    It.IsAny<Turno>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                    5, It.IsAny<CancellationToken>()))
                .ReturnsAsync((false, 5));

            // Act & Assert -> Pasamos usuarioId
            var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(
                () => _sut.CrearTurnoAsync(usuarioId, dto));

            Assert.Contains("límite", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ActivarTurnoAsync_ConTurnoPendienteVigente_DeberiaActivarlo()
        {
            int usuarioId = 1;
            int turnoId = 1;
            // Arrange
            var turno = new Turno
            {
                Id = turnoId,
                UsuarioId = usuarioId,
                Estado = EstadoTurno.Pending,
                FechaCreacionUtc = DateTime.UtcNow.AddMinutes(-5),
                FechaExpiracionUtc = DateTime.UtcNow.AddMinutes(10),
                Sucursal = new Sucursal { Nombre = "Sucursal Centro" }
            };

            _turnoRepositoryMock
                .Setup(r => r.ObtenerPorIdAsync(turnoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(turno);

            // Act
            var resultado = await _sut.ActivarTurnoAsync(usuarioId, turnoId);

            // Assert
            Assert.Equal("Active", resultado.Estado);
            Assert.NotNull(resultado.FechaActivacionUtc);

            _turnoRepositoryMock.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ActivarTurnoAsync_ConTurnoQuePasoLos15Minutos_DeberiaLanzarExcepcionYMarcarloExpirado()
        {
            int turnoId = 1;
            int usuarioId = 1;
            // Arrange
            var turno = new Turno
            {
                Id = turnoId,
                UsuarioId = usuarioId,
                Estado = EstadoTurno.Pending,
                FechaCreacionUtc = DateTime.UtcNow.AddMinutes(-20),
                FechaExpiracionUtc = DateTime.UtcNow.AddMinutes(-5),
                Sucursal = new Sucursal { Nombre = "Sucursal Centro" }
            };

            _turnoRepositoryMock
                .Setup(r => r.ObtenerPorIdAsync(turnoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(turno);

            // Act & Assert
            await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _sut.ActivarTurnoAsync(usuarioId, turnoId));

            Assert.Equal(EstadoTurno.Expired, turno.Estado);
        }

        [Fact]
        public async Task ActivarTurnoAsync_ConTurnoYaActivo_DeberiaLanzarExcepcion()
        {
            int usuarioId = 1;
            int turnoId = 1;
            // Arrange
            var turno = new Turno
            {
                Id = turnoId,
                UsuarioId = usuarioId,
                Estado = EstadoTurno.Active,
                FechaCreacionUtc = DateTime.UtcNow.AddMinutes(-5),
                FechaExpiracionUtc = DateTime.UtcNow.AddMinutes(10)
            };

            _turnoRepositoryMock
                .Setup(r => r.ObtenerPorIdAsync(turnoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(turno);

            // Act & Assert
            await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _sut.ActivarTurnoAsync(usuarioId, turnoId));
        }

        [Fact]
        public async Task ActivarTurnoAsync_ConIdInexistente_DeberiaLanzarExcepcion()
        {
            int turnoIdInexistente = 999;
            int usuarioId = 1;
            // Arrange
            _turnoRepositoryMock
                .Setup(r => r.ObtenerPorIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Turno?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _sut.ActivarTurnoAsync(usuarioId, turnoIdInexistente));
        }

        [Fact]
        public async Task ObtenerPorIdAsync_ConIdExistente_DeberiaRetornarElTurno()
        {
            // Arrange
            var turno = new Turno
            {
                Id = 1,
                UsuarioId = 1,
                Usuario = new Usuario { Id = 1, Cedula = "123456789" },
                Estado = EstadoTurno.Pending,
                Sucursal = new Sucursal { Nombre = "Sucursal Centro" }
            };

            _turnoRepositoryMock
                .Setup(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(turno);

            // Act
            var resultado = await _sut.ObtenerPorIdAsync(1);

            // Assert
            Assert.Equal(1, resultado.Id);
            Assert.Equal("123456789", resultado.Cedula);
        }

        [Fact]
        public async Task ObtenerPorIdAsync_ConIdInexistente_DeberiaLanzarExcepcion()
        {
            // Arrange
            _turnoRepositoryMock
                .Setup(r => r.ObtenerPorIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Turno?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _sut.ObtenerPorIdAsync(999));
        }

        [Fact]
        public async Task ActivarTurnoAsync_ConTurnoDeOtroUsuario_DeberiaRechazarloSinModificarlo()
        {
            var turno = new Turno
            {
                Id = 1,
                UsuarioId = 99, 
                Estado = EstadoTurno.Pending,
                FechaExpiracionUtc = DateTime.UtcNow.AddMinutes(10)
            };

            _turnoRepositoryMock
                .Setup(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(turno);

            await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _sut.ActivarTurnoAsync(usuarioId: 1, turnoId: 1));

            Assert.Equal(EstadoTurno.Pending, turno.Estado);
            _turnoRepositoryMock.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ObtenerMisTurnosDeHoyAsync_DeberiaMapearTodosLosTurnosCorrectamente()
        {
            int usuarioId = 1;
            // Arrange
            var turnos = new List<Turno>
            {
                new() { Id = 1, UsuarioId = 1, Estado = EstadoTurno.Pending, Sucursal = new Sucursal { Nombre = "A" } },
                new() { Id = 2, UsuarioId = 2, Estado = EstadoTurno.Active, Sucursal = new Sucursal { Nombre = "B" } }
            };

            _turnoRepositoryMock
                .Setup(r => r.ObtenerPorUsuarioYFechaAsync(usuarioId, It.IsAny<DateTime>()))
                .ReturnsAsync(turnos);

            // Act
            var resultado = await _sut.ObtenerMisTurnosDeHoyAsync(usuarioId);

            // Assert
            Assert.Equal(2, resultado.Count);
            Assert.Equal("Pending", resultado[0].Estado);
            Assert.Equal("Active", resultado[1].Estado);
        }

        [Fact]
        public async Task ExpirarTurnosVencidosAsync_DeberiaDelegarAlRepositorioConLaFechaActual()
        {
            // Arrange
            _turnoRepositoryMock
                .Setup(r => r.ExpirarVencidosAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(3);

            var antes = DateTime.UtcNow;

            // Act
            var resultado = await _sut.ExpirarTurnosVencidosAsync();

            // Assert
            Assert.Equal(3, resultado);
            _turnoRepositoryMock.Verify(r => r.ExpirarVencidosAsync(
                It.Is<DateTime>(d => d >= antes && d <= DateTime.UtcNow),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}