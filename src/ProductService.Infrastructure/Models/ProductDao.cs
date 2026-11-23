using ProductService.Domain.Products;

namespace ProductService.Infrastructure.Models;

public sealed class ProductDao
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required decimal Price { get; init; }

    public required double Weight { get; init; }

    public required ProductType Type { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public required long WarehouseId { get; init; }
}
