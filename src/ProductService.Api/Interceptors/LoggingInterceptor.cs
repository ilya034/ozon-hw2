using System.Diagnostics;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace ProductService.Api.Interceptors;

public sealed class LoggingInterceptor(ILogger<LoggingInterceptor> logger) : Interceptor
{
    private static readonly JsonFormatter Formatter = new(
        JsonFormatter.Settings.Default.WithFormatDefaultValues(false));

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var method = context.Method;
        var startedAt = Stopwatch.GetTimestamp();

        logger.LogInformation(
            "gRPC request {Method}: {Request}",
            method,
            Format(request));

        try
        {
            var response = await continuation(request, context).ConfigureAwait(false);

            logger.LogInformation(
                "gRPC response {Method}, {ElapsedMs} ms: {Response}",
                method,
                Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds,
                Format(response));

            return response;
        }
        catch (RpcException exception)
        {
            logger.LogWarning(
                "gRPC error {Method}, {ElapsedMs} ms: {StatusCode} {Detail}",
                method,
                Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds,
                exception.StatusCode,
                exception.Status.Detail);

            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "gRPC unhandled exception {Method} within {ElapsedMs} ms",
                method,
                Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds);

            throw;
        }
    }

    private static string Format<T>(T message) =>
        message is IMessage proto ? Formatter.Format(proto) : "<not a protobuf>";
}
