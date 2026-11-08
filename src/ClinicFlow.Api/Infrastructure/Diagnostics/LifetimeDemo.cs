namespace ClinicFlow.Api.Infrastructure.Diagnostics;

// Three identical classes, three lifetimes. Each instance gets a new id when it is created,
// so the ids show exactly when the container creates a new object.
public sealed class SingletonOperation { public Guid Id { get; } = Guid.NewGuid(); }

public sealed class ScopedOperation { public Guid Id { get; } = Guid.NewGuid(); }

public sealed class TransientOperation { public Guid Id { get; } = Guid.NewGuid(); }

/// <summary>A second consumer in the same request, to compare ids with the endpoint's own.</summary>
public sealed class LifetimeReporter(SingletonOperation singleton, ScopedOperation scoped, TransientOperation transient)
{
    public object Ids => new { singleton = singleton.Id, scoped = scoped.Id, transient = transient.Id };
}

public static class LifetimeDemoExtensions
{
    public static IServiceCollection AddLifetimeDemo(this IServiceCollection services)
    {
        services.AddSingleton<SingletonOperation>();   // one for the whole app
        services.AddScoped<ScopedOperation>();         // one per HTTP request
        services.AddTransient<TransientOperation>();   // a new one every time it is injected
        services.AddScoped<LifetimeReporter>();

        return services;
    }

    /// <summary>GET /api/diagnostics/lifetimes (Development only).</summary>
    public static IEndpointRouteBuilder MapLifetimeDemo(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/diagnostics/lifetimes", (
                SingletonOperation singleton,
                ScopedOperation scoped,
                TransientOperation transient,
                LifetimeReporter reporter) => new
                {
                    endpoint = new { singleton = singleton.Id, scoped = scoped.Id, transient = transient.Id },
                    reporter = reporter.Ids
                })
            .WithTags("Diagnostics")
            .WithSummary("Shows when Singleton, Scoped and Transient instances are created")
            .AllowAnonymous();

        return app;
    }
}
