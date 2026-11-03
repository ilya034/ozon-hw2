using Grpc.Core;
using ProductService.Api.Mapping;
using ProductService.Application.Abstractions;
using Proto = ProductService.Api.Protos;

namespace ProductService.Api.Grpc;

public sealed class ProductsGrpcService(IProductAppService appService) : Proto.ProductService.ProductServiceBase
{
    public override async Task<Proto.CreateProductResponse> CreateProduct(
        Proto.CreateProductRequest request,
        ServerCallContext context)
    {
        var id = await appService
            .CreateAsync(ProductMapping.ToApplication(request), context.CancellationToken)
            .ConfigureAwait(false);

        return new Proto.CreateProductResponse { Id = id.ToString() };
    }

    public override async Task<Proto.GetProductResponse> GetProduct(
        Proto.GetProductRequest request,
        ServerCallContext context)
    {
        var product = await appService
            .GetByIdAsync(ProductMapping.ToApplication(request), context.CancellationToken)
            .ConfigureAwait(false);

        return new Proto.GetProductResponse { Product = ProductMapping.ToProto(product) };
    }

    public override async Task<Proto.ListProductsResponse> ListProducts(
        Proto.ListProductsRequest request,
        ServerCallContext context)
    {
        var page = await appService
            .ListAsync(ProductMapping.ToApplication(request), context.CancellationToken)
            .ConfigureAwait(false);

        var response = new Proto.ListProductsResponse
        {
            TotalCount = page.TotalCount,
            Page = page.Page,
            PageSize = page.PageSize,
            TotalPages = page.TotalPages,
        };

        response.Items.AddRange(page.Items.Select(ProductMapping.ToProto));

        return response;
    }
}
