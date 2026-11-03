namespace ProductService.Domain.Exceptions;

public sealed class ProductNotFoundException(long id)
    : Exception($"Товар с идентификатором {id} не найден.")
{
    public long ProductId { get; } = id;
}