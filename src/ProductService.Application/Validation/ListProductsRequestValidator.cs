using FluentValidation;
using ProductService.Application.Models;

namespace ProductService.Application.Validation;

public sealed class ListProductsRequestValidator : AbstractValidator<ListProductsRequest>
{
    public const int MaxPageSize = 100;

    public ListProductsRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Номер страницы должен быть не меньше 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("Размер страницы должен быть не меньше 1.")
            .LessThanOrEqualTo(MaxPageSize).WithMessage($"Размер страницы не должен превышать {MaxPageSize}.");

        RuleFor(x => x.CreatedTo)
            .GreaterThanOrEqualTo(x => x.CreatedFrom)
            .When(x => x.CreatedTo.HasValue && x.CreatedFrom.HasValue)
            .WithMessage("Дата окончания периода не может быть раньше даты начала периода.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Вид товара должен быть.")
            .When(x => x.Type.HasValue);

        RuleFor(x => x.WarehouseId)
            .GreaterThan(0).When(x => x.WarehouseId.HasValue)
            .WithMessage("Номер склада должен быть больше нуля.");
    }
}
