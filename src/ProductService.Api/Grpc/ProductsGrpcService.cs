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
            .CreateAsync(request.ToApplication(), context.CancellationToken)
            .ConfigureAwait(false);

        return new Proto.CreateProductResponse { Id = id.ToString() };
    }

    public override async Task<Proto.GetProductResponse> GetProduct(
        Proto.GetProductRequest request,
        ServerCallContext context)
    {
        var product = await appService
            .GetByIdAsync(request.ToApplication(), context.CancellationToken)
            .ConfigureAwait(false);

        return new Proto.GetProductResponse { Product = product.ToProto() };
    }

    public override async Task<Proto.ListProductsResponse> ListProducts(
        Proto.ListProductsRequest request,
        ServerCallContext context)
    {
        var page = await appService
            .ListAsync(request.ToApplication(), context.CancellationToken)
            .ConfigureAwait(false);

        return page.ToProto();
    }

    public override async Task<Proto.UpdateProductResponse> UpdateProduct(
        Proto.UpdateProductRequest request,
        ServerCallContext context)
    {
        var updated = await appService
            .UpdateAsync(request.ToApplication(), context.CancellationToken)
            .ConfigureAwait(false);

        return new Proto.UpdateProductResponse { Product = updated.ToProto() };
    }
}
