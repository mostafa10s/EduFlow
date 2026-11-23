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
            string password,
            CancellationToken cancellationToken);
    }
}
