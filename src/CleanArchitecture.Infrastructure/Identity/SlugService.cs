using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.RegularExpressions;

namespace CleanArchitecture.Infrastructure.Identity;

public class SlugService : ISlugService
{
    private readonly ApplicationDbContext _context;

    public SlugService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateUniqueSlugAsync(
        string name,
        CancellationToken cancellationToken)
    {
        var slug = GenerateSlug(name);

        var uniqueSlug = slug;
        var counter = 2;

        while (await _context.Tenants
            .AnyAsync(x => x.Slug == uniqueSlug, cancellationToken))
        {
            uniqueSlug = $"{slug}-{counter}";
            counter++;
        }

        return uniqueSlug;
    }

    private static string GenerateSlug(string value)
    {
        value = value.Trim().ToLowerInvariant();

        value = Regex.Replace(value, @"\s+", "-");

        value = Regex.Replace(value, @"[^a-z0-9\-]", "");

        value = Regex.Replace(value, @"-+", "-");

        value = value.Trim('-');

        return string.IsNullOrWhiteSpace(value)
            ? $"tenant-{Guid.NewGuid():N}"[..15]
            : value;
    }
}