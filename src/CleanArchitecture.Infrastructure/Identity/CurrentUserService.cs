
using CleanArchitecture.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CleanArchitecture.Infrastructure.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User =>
        _httpContextAccessor.HttpContext?.User;

    public string? UserId =>
        User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? Email =>
        User?.FindFirstValue(ClaimTypes.Email);

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;
    public long? TenantId
    {
        get
        {
            var tenantId = User?.FindFirstValue("tenant_id");

            return long.TryParse(tenantId, out var id)
                ? id
                : null;
        }
    }

    public string? TenantRole =>
        User?.FindFirstValue("tenant_role");
}
