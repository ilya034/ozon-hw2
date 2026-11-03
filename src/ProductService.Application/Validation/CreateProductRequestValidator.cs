using FluentValidation;
using ProductService.Application.Models;
using ProductService.Domain.Products;

namespace ProductService.Application.Validation;

public sealed class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public const int NameMaxLength = 200;

    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Наименование товара обязательно.")
            .MaximumLength(NameMaxLength)
            .WithMessage($"Наименование товара не должно превышать {NameMaxLength} символов.");

        RuleFor(x => x.Price)
            .GreaterThan(0m).WithMessage("Цена товара должна быть больше нуля.");

        RuleFor(x => x.Weight)
            .GreaterThan(0d).WithMessage("Вес товара должен быть больше нуля.")
            .Must(w => !double.IsNaN(w) && !double.IsInfinity(w))
            .WithMessage("Вес товара должен быть конечным числом.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Вид товара должен быть.")
            .NotEqual(ProductType.Unspecified).WithMessage("Вид товара обязателен.");

        RuleFor(x => x.WarehouseId)
            .GreaterThan(0).WithMessage("Номер склада должен быть больше нуля.");
    }
}