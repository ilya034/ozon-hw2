namespace ProductService.Domain.Products;

public sealed class Product
{
    public long Id { get; init; }

    public required string Name { get; init; }

    public decimal Price { get; init; }

    public double Weight { get; init; }

    public ProductType Type { get; init; }

    public DateTime CreatedAt { get; init; }

    public long WarehouseId { get; init; }
}