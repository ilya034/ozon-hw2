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
            CreatedAt = DateTime.UtcNow,
        };

        repository.Add(product);
        logger.LogInformation("Created product {ProductId} ({ProductName})", product.Id, product.Name);

        return product.Id;
    }

    public async Task<PagedResult<Product>> ListAsync(
        ListProductsRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        await listValidator.ValidateAndThrowAsync(request, cancellationToken).ConfigureAwait(false);

        var result = repository.List(request.ToFilter(), request.Page, request.PageSize);
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

        return repository.GetById(request.Id) ?? throw new ProductNotFoundException(request.Id);
    }
}
