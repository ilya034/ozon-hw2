using ProductService.Domain.Products;
using ProductService.Infrastructure.Models;

namespace ProductService.Infrastructure.Mapping;

internal static class ProductDaoMapper
{
    public static Product ToDomain(this ProductDao dao)
    {
        ArgumentNullException.ThrowIfNull(dao);

        return new Product
        {
            Id = dao.Id,
            Name = dao.Name,
            Price = dao.Price,
            Weight = dao.Weight,
            Type = dao.Type,
            CreatedAt = dao.CreatedAt.ToUniversalTime(),
            WarehouseId = dao.WarehouseId
        };
    }
}
