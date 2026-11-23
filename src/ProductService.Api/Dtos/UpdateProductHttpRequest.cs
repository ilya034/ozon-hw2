using ProductService.Domain.Products;

namespace ProductService.Api.Dtos;

public sealed record UpdateProductHttpRequest(
    string? Name,
    decimal Price,
    double Weight,
    ProductType Type,
    long WarehouseId);
