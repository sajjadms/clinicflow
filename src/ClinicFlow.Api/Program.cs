using ClinicFlow.Api.Features;
using ClinicFlow.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Services in
builder.Services
    .AddInfrastructure(builder.Configuration)
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

// Endpoints mapped
app.MapFeatureEndpoints();

app.Run();
