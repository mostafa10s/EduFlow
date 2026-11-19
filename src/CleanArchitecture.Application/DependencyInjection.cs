
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Application;

/// <summary>
/// Marker type used to anchor FluentValidation assembly scanning
/// to this project, independent of static class names.
/// </summary>
public sealed class ApplicationAssemblyMarker { }

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
  

        services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyMarker>();

        return services;
    }
}
