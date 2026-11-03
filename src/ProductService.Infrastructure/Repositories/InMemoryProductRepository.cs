using System.Collections.Concurrent;
using ProductService.Application.Abstractions;
using ProductService.Application.Models;
using ProductService.Domain.Products;

namespace ProductService.Infrastructure.Repositories;

public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<Guid, Product> _products = new();

    public void Add(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);
        _products.TryAdd(product.Id, product);
    }

    public Product? GetById(Guid id)
    {
        return _products.TryGetValue(id, out var product) ? product : null;
    }

    public PagedResult<Product> List(ProductFilter filter, int page, int pageSize)
    {
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        var query = _products.Values.AsEnumerable();

        if (filter.CreatedFrom.HasValue)
        {
            var from = filter.CreatedFrom.Value;
            query = query.Where(p => p.CreatedAt >= from);
        }

        if (filter.CreatedTo.HasValue)
        {
            var to = filter.CreatedTo.Value;
            query = query.Where(p => p.CreatedAt <= to);
        }

        if (filter.Type.HasValue)
        {
            var type = filter.Type.Value;
            query = query.Where(p => p.Type == type);
        }

        if (filter.WarehouseId.HasValue)
        {
            var warehouseId = filter.WarehouseId.Value;
            query = query.Where(p => p.WarehouseId == warehouseId);
        }

        var ordered = query
            .OrderBy(p => p.CreatedAt)
            .ToList();

        var items = ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<Product>(items, ordered.Count, page, pageSize);
    }
}