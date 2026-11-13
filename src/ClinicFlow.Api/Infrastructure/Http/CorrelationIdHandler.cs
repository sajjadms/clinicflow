using System.Diagnostics;

namespace ClinicFlow.Api.Infrastructure.Http;

/// <summary>
/// Runs for every outgoing call: adds an X-Correlation-Id header (the id of the incoming request),
/// and logs the call and its result. Like middleware, but for HttpClient.
/// </summary>
public sealed class CorrelationIdHandler(IHttpContextAccessor accessor, ILogger<CorrelationIdHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var correlationId = accessor.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();
        request.Headers.Remove("X-Correlation-Id");   // a retry sends the same request again: set it once
        request.Headers.Add("X-Correlation-Id", correlationId);

        logger.LogInformation("Calling {Method} {Url}", request.Method, request.RequestUri);
        var stopwatch = Stopwatch.StartNew();

        var response = await base.SendAsync(request, cancellationToken);

        logger.LogInformation("{Url} returned {StatusCode} in {ElapsedMs} ms",
            request.RequestUri, (int)response.StatusCode, stopwatch.ElapsedMilliseconds);
        return response;
    }
}
