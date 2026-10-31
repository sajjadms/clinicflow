using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Infrastructure.ErrorHandling;

/// <summary>
/// Turns any unhandled exception into a ProblemDetails response.
/// Malformed requests (bad JSON, wrong route values) keep their 400 status,
/// and a unique-index violation in SQL Server becomes 409 Conflict.
/// </summary>
public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    // SQL Server error numbers for "duplicate key" (unique index / unique constraint).
    private static readonly int[] DuplicateKeyErrors = [2601, 2627];

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            BadHttpRequestException badRequest => (badRequest.StatusCode, "The request is invalid."),
            DbUpdateException { InnerException: SqlException sql } when DuplicateKeyErrors.Contains(sql.Number)
                => (StatusCodes.Status409Conflict, "A record with the same unique value already exists."),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception for {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails =
            {
                Status = statusCode,
                Title = title
            }
        });
    }
}
