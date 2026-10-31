using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace ClinicFlow.Api.Infrastructure.Auth;

/// <summary>Tells the OpenAPI document (and so Swagger UI) that the API accepts "Authorization: Bearer &lt;token&gt;".</summary>
internal sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Paste the accessToken from POST /api/auth/login"
            }
        };

        // Swagger UI sends the token with every request once you click Authorize.
        document.Security = [new OpenApiSecurityRequirement { [new OpenApiSecuritySchemeReference("Bearer", document)] = [] }];

        return Task.CompletedTask;
    }
}
