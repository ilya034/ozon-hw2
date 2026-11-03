using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProductService.Api.Common;
using ProductService.Domain.Exceptions;

namespace ProductService.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context).ConfigureAwait(false);
        }
        catch (ValidationException exception)
        {
            logger.LogWarning(
                "Validation error {Method} {Path}: {Errors}",
                context.Request.Method,
                context.Request.Path,
                ValidationErrorMapper.ToSummary(exception));

            await WriteAsync(context, BuildValidationProblem(context, exception)).ConfigureAwait(false);
        }
        catch (ProductNotFoundException exception)
        {
            logger.LogWarning("Product {ProductId} not found", exception.ProductId);

            await WriteAsync(context, new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Product not found",
                Detail = exception.Message,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                Instance = context.Request.Path,
            }).ConfigureAwait(false);
        }
        catch (BadHttpRequestException exception)
        {
            logger.LogWarning(exception, "Bad request");

            await WriteAsync(context, new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad request",
                Detail = exception.Message,
                Instance = context.Request.Path,
            }).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            logger.LogInformation("Operation {Method} {Path} cancelled by client", context.Request.Method, context.Request.Path);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Unhandled exception {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await WriteAsync(context, new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal server error",
                Detail = "Operation finished with error",
                Instance = context.Request.Path,
            }).ConfigureAwait(false);
        }
    }

    private static ValidationProblemDetails BuildValidationProblem(HttpContext context, ValidationException exception)
    {
        var problem = new ValidationProblemDetails(ValidationErrorMapper.ToDictionary(exception))
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation error",
            Detail = ValidationErrorMapper.ToSummary(exception),
            Instance = context.Request.Path,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        };

        problem.Extensions["traceId"] = context.TraceIdentifier;

        return problem;
    }

    private static async Task WriteAsync(HttpContext context, ProblemDetails problem)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problem, problem.GetType(), options: null, contentType: "application/problem+json")
            .ConfigureAwait(false);
    }
}
