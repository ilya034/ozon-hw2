using ProductService.Domain.Products;

namespace ProductService.Api.Contracts;

public sealed record CreateProductHttpRequest(
    string? Name,
    decimal Price,
    double Weight,
    ProductType Type,
    long WarehouseId);
