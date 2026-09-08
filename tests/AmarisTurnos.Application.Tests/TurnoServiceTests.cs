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
        private readonly TurnoService _sut; // "sut" = System Under Test, convención común

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
            var dto = new CrearTurnoDto { Cedula = "123456789", SucursalId = 1 };
            var sucursal = new Sucursal { Id = 1, Nombre = "Sucursal Centro" };

            _sucursalRepositoryMock
                .Setup(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sucursal);

            _turnoRepositoryMock
                .Setup(r => r.CrearConValidacionDeLimiteAsync(
                    It.IsAny<Turno>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                    5, It.IsAny<CancellationToken>()))
                .ReturnsAsync((true, 1));

            // Act
            var resultado = await _sut.CrearTurnoAsync(dto);

            // Assert
            Assert.Equal("123456789", resultado.Cedula);
            Assert.Equal("Sucursal Centro", resultado.SucursalNombre);
            Assert.Equal("Pending", resultado.Estado);
        }

        [Fact]
        public async Task CrearTurnoAsync_ConSucursalInexistente_DeberiaLanzarExcepcion()
        {
            // Arrange
            var dto = new CrearTurnoDto { Cedula = "123456789", SucursalId = 999 };

            _sucursalRepositoryMock
                .Setup(r => r.ObtenerPorIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Sucursal?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(
                () => _sut.CrearTurnoAsync(dto));

            Assert.Contains("sucursal", ex.Message, StringComparison.OrdinalIgnoreCase);

            // Verificamos que, al fallar antes, NUNCA se intentó crear el turno
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
            var dto = new CrearTurnoDto { Cedula = "123456789", SucursalId = 1 };
            var sucursal = new Sucursal { Id = 1, Nombre = "Sucursal Centro" };

            _sucursalRepositoryMock
                .Setup(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sucursal);

            // El repositorio simula que ya existen 5 turnos hoy -> no permite crear
            _turnoRepositoryMock
                .Setup(r => r.CrearConValidacionDeLimiteAsync(
                    It.IsAny<Turno>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                    5, It.IsAny<CancellationToken>()))
                .ReturnsAsync((false, 5));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(
                () => _sut.CrearTurnoAsync(dto));

            Assert.Contains("límite", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ActivarTurnoAsync_ConTurnoPendienteVigente_DeberiaActivarlo()
        {
            // Arrange
            var turno = new Turno
            {
                Id = 1,
                Estado = EstadoTurno.Pending,
                FechaCreacionUtc = DateTime.UtcNow.AddMinutes(-5),
                FechaExpiracionUtc = DateTime.UtcNow.AddMinutes(10), // aún no vence
                Sucursal = new Sucursal { Nombre = "Sucursal Centro" }
            };

            _turnoRepositoryMock
                .Setup(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(turno);

            // Act
            var resultado = await _sut.ActivarTurnoAsync(1);

            // Assert
            Assert.Equal("Active", resultado.Estado);
            Assert.NotNull(resultado.FechaActivacionUtc);

            _turnoRepositoryMock.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ActivarTurnoAsync_ConTurnoQuePasoLos15Minutos_DeberiaLanzarExcepcionYMarcarloExpirado()
        {
            // Arrange
            var turno = new Turno
            {
                Id = 1,
                Estado = EstadoTurno.Pending,
                FechaCreacionUtc = DateTime.UtcNow.AddMinutes(-20),
                FechaExpiracionUtc = DateTime.UtcNow.AddMinutes(-5), // ya venció hace 5 min
                Sucursal = new Sucursal { Nombre = "Sucursal Centro" }
            };

            _turnoRepositoryMock
                .Setup(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(turno);

            // Act & Assert
            await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _sut.ActivarTurnoAsync(1));

            // Confirmamos el efecto secundario esperado: el turno quedó marcado como Expired
            Assert.Equal(EstadoTurno.Expired, turno.Estado);
        }

        [Fact]
        public async Task ActivarTurnoAsync_ConTurnoYaActivo_DeberiaLanzarExcepcion()
        {
            // Arrange
            var turno = new Turno
            {
                Id = 1,
                Estado = EstadoTurno.Active, // ya estaba activado antes
                FechaCreacionUtc = DateTime.UtcNow.AddMinutes(-5),
                FechaExpiracionUtc = DateTime.UtcNow.AddMinutes(10)
            };

            _turnoRepositoryMock
                .Setup(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(turno);

            // Act & Assert
            await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _sut.ActivarTurnoAsync(1));
        }

        [Fact]
        public async Task ActivarTurnoAsync_ConIdInexistente_DeberiaLanzarExcepcion()
        {
            // Arrange
            _turnoRepositoryMock
                .Setup(r => r.ObtenerPorIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Turno?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _sut.ActivarTurnoAsync(999));
        }

        [Fact]
        public async Task ObtenerPorIdAsync_ConIdExistente_DeberiaRetornarElTurno()
        {
            // Arrange
            var turno = new Turno
            {
                Id = 1,
                Cedula = "123456789",
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
        public async Task ObtenerTodosAsync_DeberiaMapearTodosLosTurnosCorrectamente()
        {
            // Arrange
            var turnos = new List<Turno>
        {
            new() { Id = 1, Cedula = "111", Estado = EstadoTurno.Pending, Sucursal = new Sucursal { Nombre = "A" } },
            new() { Id = 2, Cedula = "222", Estado = EstadoTurno.Active, Sucursal = new Sucursal { Nombre = "B" } }
        };

            _turnoRepositoryMock
                .Setup(r => r.ObtenerTodosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(turnos);

            // Act
            var resultado = await _sut.ObtenerTodosAsync();

            // Assert
            Assert.Equal(2, resultado.Count);
            Assert.Equal("Pending", resultado[0].Estado);
            Assert.Equal("Active", resultado[1].Estado);
        }
    }
}

