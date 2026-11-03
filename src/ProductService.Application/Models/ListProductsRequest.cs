using ProductService.Domain.Products;

namespace ProductService.Application.Models;

public sealed record ListProductsRequest(
    DateTime? CreatedFrom,
    DateTime? CreatedTo,
    ProductType? Type,
    long? WarehouseId,
    int Page,
    int PageSize)
{
    public static ListProductsRequest Empty { get; } = new(null, null, null, null, 1, 20);
    public ProductFilter ToFilter() => new(CreatedFrom, CreatedTo, Type, WarehouseId);
}
