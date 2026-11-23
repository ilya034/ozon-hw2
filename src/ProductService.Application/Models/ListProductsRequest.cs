using ProductService.Domain.Products;

namespace ProductService.Application.Models;

public sealed record ListProductsRequest(
    DateTimeOffset? CreatedFrom,
    DateTimeOffset? CreatedTo,
    ProductType? Type,
    long? WarehouseId,
    int Page,
    int PageSize)
{
    public const int DefaultPage = 1;

    public const int DefaultPageSize = 20;

    public static ListProductsRequest Empty { get; } = new(null, null, null, null, DefaultPage, DefaultPageSize);

    public ProductFilter ToFilter()
    {
        var normalizedType = Type == ProductType.Unspecified ? null : Type;

        return new ProductFilter(CreatedFrom, CreatedTo, normalizedType, WarehouseId);
    }
}
