using ClinicFlow.Api.Features.Patients;
using FluentValidation;

namespace ClinicFlow.Api.Features;

/// <summary>
/// One place where every feature (vertical slice) is registered and mapped.
/// Handlers and validators register themselves by assembly scanning;
/// adding a new feature means adding one Map...Endpoints() line below.
/// </summary>
public static class FeatureExtensions
{
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        var assembly = typeof(FeatureExtensions).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }

    public static IEndpointRouteBuilder MapFeatureEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPatientEndpoints();

        return app;
    }
}
