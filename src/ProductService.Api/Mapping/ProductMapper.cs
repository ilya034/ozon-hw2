using Google.Protobuf.WellKnownTypes;
using ProductService.Api.Dtos;
using ProductService.Application.Models;
using ProductService.Domain.Products;
using Proto = ProductService.Api.Protos;

namespace ProductService.Api.Mapping;

internal static class ProductMapper
{
    public static CreateProductRequest ToApplication(this CreateProductHttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new CreateProductRequest(
            request.Name,
            request.Price,
            request.Weight,
            request.Type,
            request.WarehouseId);
    }

    public static UpdateProductRequest ToApplication(this UpdateProductHttpRequest request, Guid id)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new UpdateProductRequest(
            id,
            request.Name,
            request.Price,
            request.Weight,
            request.Type,
            request.WarehouseId);
    }

    public static ProductHttpResponse ToHttp(this Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        return new ProductHttpResponse(
            product.Id,
            product.Name,
            product.Price,
            product.Weight,
            product.Type,
            product.CreatedAt,
            product.WarehouseId);
    }

    public static PagedProductsResponse ToHttp(this PagedResult<Product> page)
    {
        ArgumentNullException.ThrowIfNull(page);

        return new PagedProductsResponse(
            page.Items.Select(ToHttp).ToList(),
            page.TotalCount,
            page.Page,
            page.PageSize,
            page.TotalPages);
    }

    public static Proto.Product ToProto(this Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        return new Proto.Product
        {
            Id = product.Id.ToString(),
            Name = product.Name,
            Price = (double)product.Price,
            Weight = product.Weight,
            Type = product.Type.ToProto(),
            CreatedAt = Timestamp.FromDateTimeOffset(product.CreatedAt),
            WarehouseId = product.WarehouseId
        };
    }

    public static Proto.ListProductsResponse ToProto(this PagedResult<Product> page)
    {
        ArgumentNullException.ThrowIfNull(page);

        var response = new Proto.ListProductsResponse
        {
            TotalCount = page.TotalCount,
            Page = page.Page,
            PageSize = page.PageSize,
            TotalPages = page.TotalPages
        };

        response.Items.AddRange(page.Items.Select(ToProto));

        return response;
    }

    public static CreateProductRequest ToApplication(this Proto.CreateProductRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new CreateProductRequest(
            request.Name,
            ToDecimal(request.Price),
            request.Weight,
            request.Type.ToDomain(),
            request.WarehouseId);
    }

    public static GetProductRequest ToApplication(this Proto.GetProductRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new GetProductRequest(Guid.TryParse(request.Id, out var id) ? id : Guid.Empty);
    }

    public static ListProductsRequest ToApplication(this Proto.ListProductsRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new ListProductsRequest(
            request.CreatedFrom?.ToDateTimeOffset(),
            request.CreatedTo?.ToDateTimeOffset(),
            request.HasType ? request.Type.ToDomain() : null,
            request.HasWarehouseId ? request.WarehouseId : null,
            request.HasPage ? request.Page : ListProductsRequest.DefaultPage,
            request.HasPageSize ? request.PageSize : ListProductsRequest.DefaultPageSize);
    }

    public static UpdateProductRequest ToApplication(this Proto.UpdateProductRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new UpdateProductRequest(
            Guid.TryParse(request.Id, out var id) ? id : Guid.Empty,
            request.Name,
            ToDecimal(request.Price),
            request.Weight,
            request.Type.ToDomain(),
            request.WarehouseId);
    }

    public static Proto.ProductType ToProto(this ProductType type)
    {
        return type switch
        {
            ProductType.General => Proto.ProductType.General,
            ProductType.HouseholdChemicals => Proto.ProductType.HouseholdChemicals,
            ProductType.Electronics => Proto.ProductType.Electronics,
            ProductType.Food => Proto.ProductType.Food,
            _ => Proto.ProductType.Unspecified
        };
    }

    public static ProductType ToDomain(this Proto.ProductType type)
    {
        return type switch
        {
            Proto.ProductType.General => ProductType.General,
            Proto.ProductType.HouseholdChemicals => ProductType.HouseholdChemicals,
            Proto.ProductType.Electronics => ProductType.Electronics,
            Proto.ProductType.Food => ProductType.Food,
            _ => ProductType.Unspecified
        };
    }

    private static decimal ToDecimal(double value)
    {
        if (!double.IsFinite(value) || value > (double)decimal.MaxValue || value < (double)decimal.MinValue)
        {
            return 0m;
        }

        return (decimal)value;
    }
}
