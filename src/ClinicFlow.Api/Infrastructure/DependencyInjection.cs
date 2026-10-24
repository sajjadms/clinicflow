using ClinicFlow.Api.Infrastructure.ErrorHandling;
using ClinicFlow.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // In-memory for now; Video 2 swaps this for SQL Server using configuration.
        services.AddDbContext<ClinicFlowDbContext>(options =>
            options.UseInMemoryDatabase("ClinicFlow"));

        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }

    /// <summary>
    /// Built-in OpenAPI document generation (.NET 9+). Swagger UI renders it; see Program.cs.
    /// </summary>
    public static IServiceCollection AddApiDocs(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new()
                {
                    Title = "ClinicFlow API",
                    Version = "v1",
                    Description = "Patients, doctors and appointments for outpatient clinics."
                };

                return Task.CompletedTask;
            }));

        return services;
    }
}
