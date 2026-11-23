using FluentValidation;
using ProductService.Application.Models;

namespace ProductService.Application.Validation;

public sealed class GetProductRequestValidator : AbstractValidator<GetProductRequest>
{
    public GetProductRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Идентификатор товара обязателен.");
    }
}