using ClinicFlow.Api.Features;
using ClinicFlow.Api.Infrastructure;
using ClinicFlow.Api.Infrastructure.Auth;
using ClinicFlow.Api.Infrastructure.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();   // OpenAPI document at /openapi/v1.json
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "ClinicFlow API v1"));   // UI at /swagger

    app.MapLifetimeDemo();   // GET /api/diagnostics/lifetimes: Singleton vs Scoped vs Transient
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
