using Npgsql;

namespace ProductService.Infrastructure.Helpers;

public class PostgresConnectionFactory(string connectionString) : IPostgresConnectionFactory
{
    public NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(connectionString);
    }
}
