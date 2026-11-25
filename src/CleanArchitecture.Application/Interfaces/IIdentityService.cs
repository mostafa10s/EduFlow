using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<string> CreateUserAsync(
            string fullName,
            string email,
            string phoneNumber,
            string password,
            CancellationToken cancellationToken);

        Task<string?> GetTenantRoleAsync(
    string userId,
    long tenantId,
    CancellationToken cancellationToken);
        Task<IList<string>> GetUserRolesAsync(string userId);
    }
}
