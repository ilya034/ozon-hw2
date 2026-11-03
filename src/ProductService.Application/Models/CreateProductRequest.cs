using ProductService.Domain.Products;

namespace ProductService.Application.Models;

public sealed record CreateProductRequest(
    string? Name,
    decimal Price,
    double Weight,
    ProductType Type,
    long WarehouseId);