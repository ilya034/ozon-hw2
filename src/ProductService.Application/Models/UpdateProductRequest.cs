using ProductService.Domain.Products;

namespace ProductService.Application.Models;

public sealed record UpdateProductRequest(
    Guid Id,
    string? Name,
    decimal Price,
    double Weight,
    ProductType Type,
    long WarehouseId);
