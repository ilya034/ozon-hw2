using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Abstractions;

namespace ProductService.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<IProductAppService>(includeInternalTypes: true);
        services.AddScoped<IProductAppService, ProductAppService>();

        return services;
    }
}
