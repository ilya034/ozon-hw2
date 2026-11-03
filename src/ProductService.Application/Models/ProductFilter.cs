using ProductService.Domain.Products;

namespace ProductService.Application.Models;

public sealed record ProductFilter(
    DateTime? CreatedFrom,
    DateTime? CreatedTo,
    ProductType? Type,
    long? WarehouseId);