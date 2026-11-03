using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Abstractions;
using ProductService.Infrastructure.Repositories;

namespace ProductService.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IProductRepository, InMemoryProductRepository>();

        return services;
    }
}
