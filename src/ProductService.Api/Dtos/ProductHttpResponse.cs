using ProductService.Domain.Products;

namespace ProductService.Api.Dtos;

public sealed record ProductHttpResponse(
    Guid Id,
    string Name,
    decimal Price,
    double Weight,
    ProductType Type,
    DateTimeOffset CreatedAt,
    long WarehouseId);