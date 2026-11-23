using ProductService.Application.Models;
using ProductService.Domain.Products;

namespace ProductService.Application.Abstractions;

public interface IProductRepository
{
    Task<Guid> AddAsync(Product product, CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<Product>> ListAsync(
        ProductFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Product?> UpdateAsync(Product product, CancellationToken cancellationToken = default);
}
