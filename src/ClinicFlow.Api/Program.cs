using ClinicFlow.Api.Features;
using ClinicFlow.Api.Infrastructure;
using ClinicFlow.Api.Infrastructure.Auth;
using ClinicFlow.Api.Infrastructure.Diagnostics;
using ClinicFlow.Api.Infrastructure.Logging;
using Serilog;
using Serilog.Debugging;

// If a sink fails (for example Azure is unreachable), Serilog reports it here instead of throwing.
SelfLog.Enable(message => File.AppendAllText("logs/serilog-errors.txt", message + Environment.NewLine));

// A small logger for startup, so even a crash before the app is built gets logged.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting ClinicFlow API");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog: Console + File (appsettings.json) + Azure Blob (user-secrets)
    builder.AddSerilogLogging();

    // Services in
    builder.Services
        .AddInfrastructure(builder.Configuration)
        .AddJwtAuthentication(builder.Configuration)
        .AddApiDocs()
        .AddFeatures()
        .AddLifetimeDemo();

    var app = builder.Build();

    // Pipeline out
    app.UseExceptionHandler();

    // One log line per request: method, path, status code and time taken.
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();   // OpenAPI document at /openapi/v1.json
        app.UseSwaggerUI(options =>
            options.SwaggerEndpoint("/openapi/v1.json", "ClinicFlow API v1"));   // UI at /swagger

        app.MapLifetimeDemo();   // GET /api/diagnostics/lifetimes: Singleton vs Scoped vs Transient
        app.MapLoggingDemo();    // GET /api/diagnostics/error: an Error log on purpose
    }
    else
    {
        app.UseHttpsRedirection();
    }

    // Who are you? (reads the Bearer token)  ->  Are you allowed?
    app.UseAuthentication();
    app.UseAuthorization();

    // Endpoints mapped
    app.MapFeatureEndpoints();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)   // EF tools stop the host on purpose
{
    Log.Fatal(ex, "ClinicFlow API stopped unexpectedly");
}
finally
{
    // Writes any logs still waiting in memory (important for File and Azure Blob).
    Log.CloseAndFlush();
}
