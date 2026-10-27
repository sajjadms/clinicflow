using ClinicFlow.Api.Infrastructure.Endpoints;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ClinicFlow.Api.Features;

/// <summary>
/// One place where every feature (vertical slice) is registered and mapped.
/// Handlers, validators and endpoints all register themselves by assembly scanning.
/// </summary>
public static class FeatureExtensions
{
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        var assembly = typeof(FeatureExtensions).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);

        var endpoints = assembly.DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } && type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type));

        services.TryAddEnumerable(endpoints);

        return services;
    }

    public static IEndpointRouteBuilder MapFeatureEndpoints(this IEndpointRouteBuilder app)
    {
        foreach (var endpoint in app.ServiceProvider.GetRequiredService<IEnumerable<IEndpoint>>())
        {
            endpoint.MapEndpoint(app);
        }

        return app;
    }
}
