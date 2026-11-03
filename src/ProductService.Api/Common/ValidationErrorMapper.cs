using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;

namespace ProductService.Api.Common;

internal static class ValidationErrorMapper
{
    public const string ValidationErrorsKey = "validation-errors";

    public static Dictionary<string, string[]> ToDictionary(IEnumerable<ValidationFailure> failures)
    {
        ArgumentNullException.ThrowIfNull(failures);

        return failures
            .GroupBy(failure => string.IsNullOrWhiteSpace(failure.PropertyName)
                ? "request"
                : JsonNamingPolicy.CamelCase.ConvertName(failure.PropertyName))
            .ToDictionary(
                group => group.Key,
                group => group.Select(failure => failure.ErrorMessage).ToArray(),
                StringComparer.Ordinal);
    }

    public static Dictionary<string, string[]> ToDictionary(ValidationException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        return ToDictionary(exception.Errors);
    }

    public static string ToSummary(ValidationException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        var messages = exception.Errors
            .Select(failure => string.IsNullOrWhiteSpace(failure.PropertyName)
                ? failure.ErrorMessage
                : $"{failure.PropertyName}: {failure.ErrorMessage}")
            .Distinct(StringComparer.Ordinal);

        return string.Join("; ", messages);
    }
}
