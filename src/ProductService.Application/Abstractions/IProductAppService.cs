using ProductService.Application.Models;
using ProductService.Domain.Products;

namespace ProductService.Application.Abstractions;

public interface IProductAppService
{
    Task<Guid> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);

    Task<PagedResult<Product>> ListAsync(ListProductsRequest request, CancellationToken cancellationToken = default);

    Task<Product> GetByIdAsync(GetProductRequest request, CancellationToken cancellationToken = default);
}
