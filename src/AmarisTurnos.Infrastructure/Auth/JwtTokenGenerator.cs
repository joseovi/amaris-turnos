using AmarisTurnos.Application.Interfaces;
using AmarisTurnos.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AmarisTurnos.Infrastructure.Auth;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTime ExpiraUtc) GenerarToken(Usuario usuario)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expirationInMinutes = int.Parse(jwtSettings["ExpirationInMinutes"]!);
        var expiraUtc = DateTime.UtcNow.AddMinutes(expirationInMinutes);

        var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
        new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
        new(ClaimTypes.Name, usuario.Nombre),
        new("cedula", usuario.Cedula),
        new(ClaimTypes.Role, usuario.Rol.ToString()),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiraUtc,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraUtc);    
    }
}
