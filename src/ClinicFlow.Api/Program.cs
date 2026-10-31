using ClinicFlow.Api.Features;
using ClinicFlow.Api.Infrastructure;
using ClinicFlow.Api.Infrastructure.Auth;

var builder = WebApplication.CreateBuilder(args);

// Services in
builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddApiDocs()
    .AddFeatures();

var app = builder.Build();

// Pipeline out
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();   // OpenAPI document at /openapi/v1.json
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "ClinicFlow API v1"));   // UI at /swagger
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
