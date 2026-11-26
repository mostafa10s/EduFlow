namespace CleanArchitecture.Application.Interfaces;

public interface ISlugService
{
    Task<string> GenerateUniqueSlugAsync(
        string name,
        CancellationToken cancellationToken);
}