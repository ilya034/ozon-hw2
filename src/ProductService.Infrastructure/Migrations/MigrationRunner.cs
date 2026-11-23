using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ProductService.Infrastructure.Migrations;

public static class MigrationRunner
{
    public static IHost RunMigrations(this IHost host)
    {
        ArgumentNullException.ThrowIfNull(host);

        var configuration = host.Services.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");
        }

        RunMigrations(connectionString);

        return host;
    }

    public static void RunMigrations(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        using var serviceProvider = CreateServiceProvider(connectionString);
        using var scope = serviceProvider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    private static ServiceProvider CreateServiceProvider(string connectionString)
    {
        return new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(builder => builder
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(MigrationRunner).Assembly).For.Migrations()
                .ConfigureGlobalProcessorOptions(options => options.ProviderSwitches = "Force Quote=false"))
            .AddLogging(log => log.AddFluentMigratorConsole())
            .BuildServiceProvider(false);
    }
}
