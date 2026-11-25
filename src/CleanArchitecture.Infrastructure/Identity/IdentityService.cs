
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<string> CreateUserAsync(
        string fullName,
        string email,
        string phoneNumber,
        string password,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await using var transaction =
            await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = new ApplicationUser
            {
                FullName = fullName,
                Email = email,
                UserName = email,
                PhoneNumber = phoneNumber
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .GroupBy(x => x.Code)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(x => x.Description).ToArray());

                throw new BadRequestException(
                    string.Join(", ", errors.SelectMany(x => x.Value)));
            }

            var profile = new UserProfile
            {
                UserId = user.Id
            };

            await _context.UserProfiles.AddAsync(
                profile,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);


            return user.Id;
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }

    }
    public async Task<string?> GetTenantRoleAsync(
    string userId,
    long tenantId,
    CancellationToken cancellationToken)
    {
        var membership = await _context.TenantMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.UserId == userId
                     && x.TenantId == tenantId
                     && x.IsActive,
                cancellationToken);

        return membership?.Role.ToString();
    }
    public async Task<IList<string>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            throw new NotFoundException("User not found.");

        return await _userManager.GetRolesAsync(user);
    }
}
