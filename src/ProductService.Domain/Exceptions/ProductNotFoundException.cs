namespace ProductService.Domain.Exceptions;

public sealed class ProductNotFoundException(Guid id)
    : Exception($"Товар с идентификатором {id} не найден.")
{
    public Guid ProductId { get; } = id;
}
