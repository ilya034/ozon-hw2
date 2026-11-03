using ProductService.Application.Models;
using ProductService.Domain.Products;

namespace ProductService.Application.Abstractions;

public interface IProductRepository
{
    long NextId();

    void Add(Product product);

    Product? GetById(long id);

    PagedResult<Product> List(ProductFilter filter, int page, int pageSize);
}
