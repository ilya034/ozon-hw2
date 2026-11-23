using ProductService.Domain.Products;

namespace ProductService.Application.Models;

public sealed record ProductFilter(
    DateTimeOffset? CreatedFrom,
    DateTimeOffset? CreatedTo,
    ProductType? Type,
    long? WarehouseId);
