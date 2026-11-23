using ProductService.Domain.Products;

namespace ProductService.Api.Dtos;

public sealed record CreateProductHttpRequest(
    string? Name,
    decimal Price,
    double Weight,
    ProductType Type,
    long WarehouseId);