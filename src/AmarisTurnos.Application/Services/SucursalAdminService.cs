using AmarisTurnos.Application.DTOs;
using AmarisTurnos.Application.Interfaces;
using AmarisTurnos.Domain.Entities;
using AmarisTurnos.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AmarisTurnos.Application.Services
{
    public class SucursalAdminService : ISucursalAdminService
    {
        private readonly ISucursalRepository _sucursalRepository;

        public SucursalAdminService(ISucursalRepository sucursalRepository)
        {
            _sucursalRepository = sucursalRepository;
        }

        public async Task<SucursalDto> CrearAsync(CrearSucursalDto dto, CancellationToken ct = default)
        {
            var sucursal = new Sucursal
            {
                Nombre = dto.Nombre,
                Direccion = dto.Direccion,
                Ciudad = dto.Ciudad,
                Activo = true
            };

            await _sucursalRepository.CrearAsync(sucursal, ct);
            return MapearADto(sucursal);
        }

        public async Task<SucursalDto> EditarAsync(int id, EditarSucursalDto dto, CancellationToken ct = default)
        {
            var sucursal = await _sucursalRepository.ObtenerPorIdAsync(id, ct)
                ?? throw new ReglaDeNegocioException("La sucursal no existe.");

            sucursal.Nombre = dto.Nombre;
            sucursal.Direccion = dto.Direccion;
            sucursal.Ciudad = dto.Ciudad;

            await _sucursalRepository.GuardarCambiosAsync(ct);
            return MapearADto(sucursal);
        }

        public async Task<SucursalDto> CambiarEstadoAsync(int id, bool activo, CancellationToken ct = default)
        {
            var sucursal = await _sucursalRepository.ObtenerPorIdAsync(id, ct)
                ?? throw new ReglaDeNegocioException("La sucursal no existe.");

            sucursal.Activo = activo;
            await _sucursalRepository.GuardarCambiosAsync(ct);
            return MapearADto(sucursal);
        }

        public async Task EliminarAsync(int id, CancellationToken ct = default)
        {
            var sucursal = await _sucursalRepository.ObtenerPorIdAsync(id, ct)
                ?? throw new ReglaDeNegocioException("La sucursal no existe.");

            try
            {
                await _sucursalRepository.EliminarAsync(sucursal, ct);
            }
            catch (DbUpdateException)
            {
                throw new ReglaDeNegocioException(
                    "No se puede eliminar una sucursal con turnos asociados. Desactívala en su lugar.");
            }
        }

        private static SucursalDto MapearADto(Sucursal s) => new()
        {
            Id = s.Id,
            Nombre = s.Nombre,
            Direccion = s.Direccion,
            Ciudad = s.Ciudad,
            Activo = s.Activo
        };
    }
}
