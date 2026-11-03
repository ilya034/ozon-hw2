using ProductService.Application.Models;
using ProductService.Domain.Products;

namespace ProductService.Application.Abstractions;

public interface IProductRepository
{
    void Add(Product product);

    Product? GetById(Guid id);

    PagedResult<Product> List(ProductFilter filter, int page, int pageSize);
}