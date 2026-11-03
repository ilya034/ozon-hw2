using Google.Protobuf.WellKnownTypes;
using ProductService.Application.Models;
using ProductService.Domain.Products;
using Proto = ProductService.Api.Protos;

namespace ProductService.Api.Mapping;

internal static class ProductMapping
{
    public const int DefaultPage = 1;

    public const int DefaultPageSize = 20;

    public static Proto.Product ToProto(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        return new Proto.Product
        {
            Id = product.Id.ToString(),
            Name = product.Name,
            Price = (double)product.Price,
            Weight = product.Weight,
            Type = ToProto(product.Type),
            CreatedAt = Timestamp.FromDateTime(ToUtc(product.CreatedAt)),
            WarehouseId = product.WarehouseId
        };
    }

    public static CreateProductRequest ToApplication(Proto.CreateProductRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new CreateProductRequest(
            request.Name,
            ToDecimal(request.Price),
            request.Weight,
            ToDomain(request.Type),
            request.WarehouseId);
    }

    public static GetProductRequest ToApplication(Proto.GetProductRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new GetProductRequest(Guid.TryParse(request.Id, out var id) ? id : Guid.Empty);
    }

    public static ListProductsRequest ToApplication(Proto.ListProductsRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new ListProductsRequest(
            request.CreatedFrom is not null ? request.CreatedFrom.ToDateTime().ToUniversalTime() : null,
            request.CreatedTo is not null ? request.CreatedTo.ToDateTime().ToUniversalTime() : null,
            request.HasType ? ToDomain(request.Type) : null,
            request.HasWarehouseId ? request.WarehouseId : null,
            request.HasPage ? request.Page : DefaultPage,
            request.HasPageSize ? request.PageSize : DefaultPageSize);
    }

    public static Proto.ProductType ToProto(ProductType type)
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

    public static ProductType ToDomain(Proto.ProductType type)
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

    private static DateTime ToUtc(DateTime value)
    {
        return DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    private static decimal ToDecimal(double value)
    {
        if (!double.IsFinite(value) || value > (double)decimal.MaxValue || value < (double)decimal.MinValue) return 0m;

        return (decimal)value;
    }
}