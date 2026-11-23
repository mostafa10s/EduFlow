
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Application;

public sealed class ApplicationAssemblyMarker { }

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
  

        services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyMarker>();
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblyContaining<ApplicationAssemblyMarker>();
        });

        services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyMarker>();

        return services;

        
    }
}
