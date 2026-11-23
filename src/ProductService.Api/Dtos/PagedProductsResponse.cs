namespace ProductService.Api.Dtos;

public sealed record PagedProductsResponse(
    IReadOnlyList<ProductHttpResponse> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);