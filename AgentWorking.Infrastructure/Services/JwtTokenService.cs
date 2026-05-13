using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AgentWorking.Infrastructure.Services;

public class JwtTokenService(IConfiguration config) : ITokenService
{
    private readonly string _key = config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured.");
    private readonly string _issuer = config["Jwt:Issuer"] ?? "portal-agro-api";
    private readonly string _audience = config["Jwt:Audience"] ?? "portal-agro-client";
    private readonly int _expiresHours = int.TryParse(config["Jwt:ExpiresInHours"], out var h) ? h : 8;

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Nome),
            new Claim(ClaimTypes.Role, user.Tipo.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: GetExpiry(),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public DateTime GetExpiry() => DateTime.UtcNow.AddHours(_expiresHours);
}
