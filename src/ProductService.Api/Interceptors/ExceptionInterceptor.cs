using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;
using ProductService.Api.Common;
using ProductService.Domain.Exceptions;

namespace ProductService.Api.Interceptors;

public sealed class ExceptionInterceptor(ILogger<ExceptionInterceptor> logger) : Interceptor
{
    private static readonly JsonSerializerOptions TrailerJsonOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context).ConfigureAwait(false);
        }
        catch (ValidationException exception)
        {
            logger.LogWarning(
                "Validation error in {Method}: {Errors}",
                context.Method,
                ValidationErrorMapper.ToSummary(exception));

            context.ResponseTrailers.Add(
                ValidationErrorMapper.ValidationErrorsKey + "-bin",
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(
                    ValidationErrorMapper.ToDictionary(exception),
                    TrailerJsonOptions)));

            throw new RpcException(new Status(StatusCode.InvalidArgument, ValidationErrorMapper.ToSummary(exception)));
        }
        catch (ProductNotFoundException exception)
        {
            logger.LogWarning("Product {ProductId} not found in {Method}", exception.ProductId, context.Method);

            throw new RpcException(new Status(StatusCode.NotFound, exception.Message));
        }
        catch (RpcException)
        {
            throw;
        }
        catch (OperationCanceledException) when (context.CancellationToken.IsCancellationRequested)
        {
            throw new RpcException(new Status(StatusCode.Cancelled, "Operation cancelled"));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception in {Method}", context.Method);

            throw new RpcException(new Status(StatusCode.Internal, "Internal server error."));
        }
    }
}