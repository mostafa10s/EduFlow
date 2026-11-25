
namespace CleanArchitecture.Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(
        string userId,
        string email,
        IList<string> roles,
        long? tenantId = null,
        string? tenantRole = null);
}
