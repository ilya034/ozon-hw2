using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Abstractions;
using ProductService.Infrastructure.Helpers;
using ProductService.Infrastructure.Repositories;

namespace ProductService.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        services.AddSingleton<IPostgresConnectionFactory>(new PostgresConnectionFactory(connectionString));
        services.AddScoped<IProductRepository, PostgresProductRepository>();

        return services;
    }
}
