using System.Text.Json.Serialization;
using ClinicFlow.Api.Infrastructure.ErrorHandling;
using ClinicFlow.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // The connection string is never committed: in development it comes from dotnet user-secrets,
        // in production from an environment variable (ConnectionStrings__ClinicFlow).
        var connectionString = configuration.GetConnectionString("ClinicFlow")
            ?? throw new InvalidOperationException(
                "Connection string 'ClinicFlow' is missing. Set it with: dotnet user-secrets set \"ConnectionStrings:ClinicFlow\" \"...\"");

        services.AddDbContext<ClinicFlowDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Enums travel as names in JSON: "gender": "Female", not 2.
        services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.AddProblemDetails();

        // .NET 10 built-in validation for minimal API parameters (DataAnnotations, e.g. on ListPatientsRequest).
        services.AddValidation();
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
