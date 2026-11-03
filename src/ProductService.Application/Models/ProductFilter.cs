using ProductService.Domain.Products;

public sealed record ProductFilter(
    DateTime? CreatedFrom,
    DateTime? CreatedTo,
    ProductType? Type,
    long? WarehouseId);
