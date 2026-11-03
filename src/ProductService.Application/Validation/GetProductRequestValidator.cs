using FluentValidation;
using ProductService.Application.Models;

namespace ProductService.Application.Validation;

public sealed class GetProductRequestValidator : AbstractValidator<GetProductRequest>
{
    public GetProductRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Идентификатор товара должен быть больше нуля.");
    }
}
