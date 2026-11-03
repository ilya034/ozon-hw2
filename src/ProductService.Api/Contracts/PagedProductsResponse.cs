namespace ProductService.Api.Contracts;

public sealed record PagedProductsResponse(
    IReadOnlyList<ProductHttpResponse> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);
