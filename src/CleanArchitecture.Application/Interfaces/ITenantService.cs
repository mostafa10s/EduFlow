using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.Interfaces;

public interface ITenantService
{
    Task<long> CreateTenantAsync(
        string name,
        string slug,
        TenantType type,
        string ownerUserId,
        CancellationToken cancellationToken);
    Task<IList<Tenant>> GetMyTenantsAsync(
    string userId,
    CancellationToken cancellationToken);
}