using FluentValidation;
using Microsoft.Extensions.Logging;
using ProductService.Application.Abstractions;
using ProductService.Application.Models;
using ProductService.Domain.Exceptions;
using ProductService.Domain.Products;

namespace ProductService.Application;

public sealed class ProductAppService(
    IProductRepository repository,
    IValidator<CreateProductRequest> createValidator,
    IValidator<ListProductsRequest> listValidator,
    IValidator<GetProductRequest> getValidator,
    IValidator<UpdateProductRequest> updateValidator,
    ILogger<ProductAppService> logger) : IProductAppService
{
    public async Task<Guid> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        await createValidator.ValidateAndThrowAsync(request, cancellationToken).ConfigureAwait(false);

        var product = new Product
        {
            Id = Guid.CreateVersion7(),
            Name = request.Name!.Trim(),
            Price = request.Price,
            Weight = request.Weight,
            Type = request.Type,
            WarehouseId = request.WarehouseId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var id = await repository.AddAsync(product, cancellationToken).ConfigureAwait(false);
        logger.LogInformation("Created product {ProductId} ({ProductName})", id, product.Name);

        return id;
    }

    public async Task<PagedResult<Product>> ListAsync(
        ListProductsRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        await listValidator.ValidateAndThrowAsync(request, cancellationToken).ConfigureAwait(false);

        var result = await repository
            .ListAsync(request.ToFilter(), request.Page, request.PageSize, cancellationToken)
            .ConfigureAwait(false);

        logger.LogInformation(
            "List of products: page {Page}, page size {PageSize}, total found {TotalCount}",
            request.Page,
            request.PageSize,
            result.TotalCount);

        return result;
    }

    public async Task<Product> GetByIdAsync(GetProductRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        await getValidator.ValidateAndThrowAsync(request, cancellationToken).ConfigureAwait(false);

        return await repository.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false)
               ?? throw new ProductNotFoundException(request.Id);
    }

    public async Task<Product> UpdateAsync(UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        await updateValidator.ValidateAndThrowAsync(request, cancellationToken).ConfigureAwait(false);

        var product = new Product
        {
            Id = request.Id,
            Name = request.Name!.Trim(),
            Price = request.Price,
            Weight = request.Weight,
            Type = request.Type,
            WarehouseId = request.WarehouseId
        };

        var updated = await repository.UpdateAsync(product, cancellationToken).ConfigureAwait(false)
                      ?? throw new ProductNotFoundException(request.Id);

        logger.LogInformation("Updated product {ProductId} ({ProductName})", updated.Id, updated.Name);

        return updated;
    }
}
