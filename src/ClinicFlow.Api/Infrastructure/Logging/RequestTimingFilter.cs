using System.Diagnostics;

namespace ClinicFlow.Api.Infrastructure.Logging;

/// <summary>
/// Group-level endpoint filter: logs every request with its status code and duration.
/// </summary>
public sealed class RequestTimingFilter(ILogger<RequestTimingFilter> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.HttpContext.Request;
        var stopwatch = Stopwatch.StartNew();

        var result = await next(context);

        var statusCode = result is IStatusCodeHttpResult { StatusCode: { } code } ? code : context.HttpContext.Response.StatusCode;

        logger.LogInformation("{Method} {Path} returned {StatusCode} in {ElapsedMs} ms",
            request.Method, request.Path, statusCode, stopwatch.ElapsedMilliseconds);

        return result;
    }
}
