
using CleanArchitecture.Application.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CleanArchitecture.Infrastructure.Identity;

public class JwtTokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    public Task<string> GenerateTokenAsync(
        string userId,
        string email,
        IList<string> roles,
        long? tenantId = null,
        string? tenantRole = null)
    {
        // Tenant ID and Tenant Role must be provided together.
        if (tenantId.HasValue != !string.IsNullOrWhiteSpace(tenantRole))
        {
            throw new ArgumentException(
                "Tenant ID and Tenant Role must be provided together.");
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Global roles (e.g. SuperAdmin).
        claims.AddRange(
            roles.Select(role =>
                new Claim(ClaimTypes.Role, role)));

        // Tenant context.
        if (tenantId.HasValue)
        {
            claims.Add(new Claim(
                "tenant_id",
                tenantId.Value.ToString()));

            claims.Add(new Claim(
                "tenant_role",
                tenantRole!));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Secret));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                _jwtSettings.ExpiryMinutes),
            signingCredentials: credentials);

        return Task.FromResult(
            new JwtSecurityTokenHandler().WriteToken(token));
    }
}
