using ProductService.Domain.Products;

namespace ProductService.Api.Contracts;

public sealed record ProductHttpResponse(
    Guid Id,
    string Name,
    decimal Price,
    double Weight,
    ProductType Type,
    DateTime CreatedAt,
    long WarehouseId);
