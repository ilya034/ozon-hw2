using Dapper;
using ProductService.Application.Abstractions;
using ProductService.Application.Models;
using ProductService.Domain.Products;
using ProductService.Infrastructure.Helpers;
using ProductService.Infrastructure.Mapping;
using ProductService.Infrastructure.Models;

namespace ProductService.Infrastructure.Repositories;

public class PostgresProductRepository(IPostgresConnectionFactory connectionFactory) : IProductRepository
{
    public async Task<Guid> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product);

        const string sql = """
                           INSERT INTO product (id, name, price, weight, type, createdat, warehouseid)
                           VALUES (@Id, @Name, @Price, @Weight, @Type, @CreatedAt, @WarehouseId)
                           RETURNING id;
                           """;

        await using var connection = connectionFactory.GetConnection();
        var command = new CommandDefinition(
            sql,
            new
            {
                product.Id,
                product.Name,
                product.Price,
                product.Weight,
                Type = (int)product.Type,
                CreatedAt = product.CreatedAt.ToUniversalTime(),
                product.WarehouseId
            },
            cancellationToken: cancellationToken);

        return await connection.QuerySingleAsync<Guid>(command).ConfigureAwait(false);
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = """
                           SELECT id, name, price, weight, type, createdat, warehouseid
                           FROM product
                           WHERE id = @Id;
                           """;

        await using var connection = connectionFactory.GetConnection();
        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        var dao = await connection.QueryFirstOrDefaultAsync<ProductDao>(command).ConfigureAwait(false);

        return dao?.ToDomain();
    }

    public async Task<PagedResult<Product>> ListAsync(
        ProductFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        if (filter.CreatedFrom.HasValue)
        {
            conditions.Add("createdat >= @CreatedFrom");
            parameters.Add("CreatedFrom", filter.CreatedFrom.Value.ToUniversalTime());
        }

        if (filter.CreatedTo.HasValue)
        {
            conditions.Add("createdat <= @CreatedTo");
            parameters.Add("CreatedTo", filter.CreatedTo.Value.ToUniversalTime());
        }

        if (filter.Type is { } type && type != ProductType.Unspecified)
        {
            conditions.Add("type = @Type");
            parameters.Add("Type", (int)type);
        }

        if (filter.WarehouseId.HasValue)
        {
            conditions.Add("warehouseid = @WarehouseId");
            parameters.Add("WarehouseId", filter.WarehouseId.Value);
        }

        var whereClause = conditions.Count > 0
            ? $"WHERE {string.Join(" AND ", conditions)}"
            : string.Empty;

        parameters.Add("Limit", pageSize);
        parameters.Add("Offset", (page - 1) * pageSize);

        var sql = $"""
                   SELECT COUNT(*)
                   FROM product
                   {whereClause};

                   SELECT id, name, price, weight, type, createdat, warehouseid
                   FROM product
                   {whereClause}
                   ORDER BY createdat, id
                   LIMIT @Limit OFFSET @Offset;
                   """;

        await using var connection = connectionFactory.GetConnection();
        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        await using var multi = await connection.QueryMultipleAsync(command).ConfigureAwait(false);

        var totalCount = await multi.ReadSingleAsync<int>().ConfigureAwait(false);
        var items = (await multi.ReadAsync<ProductDao>().ConfigureAwait(false))
            .Select(dao => dao.ToDomain())
            .ToList();

        return new PagedResult<Product>(items, totalCount, page, pageSize);
    }

    public async Task<Product?> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product);

        const string sql = """
                           UPDATE product
                           SET name = @Name,
                               price = @Price,
                               weight = @Weight,
                               type = @Type,
                               warehouseid = @WarehouseId
                           WHERE id = @Id
                           RETURNING id, name, price, weight, type, createdat, warehouseid;
                           """;

        await using var connection = connectionFactory.GetConnection();
        var command = new CommandDefinition(
            sql,
            new
            {
                product.Id,
                product.Name,
                product.Price,
                product.Weight,
                Type = (int)product.Type,
                product.WarehouseId
            },
            cancellationToken: cancellationToken);

        var dao = await connection.QueryFirstOrDefaultAsync<ProductDao>(command).ConfigureAwait(false);

        return dao?.ToDomain();
    }
}
