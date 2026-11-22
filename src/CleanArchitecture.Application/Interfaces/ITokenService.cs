using System.Security.Claims;

namespace CleanArchitecture.Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(
           string userId,
           string email,
           IList<string> roles);
}
