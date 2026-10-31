using System.Security.Claims;
using ClinicFlow.Api.Infrastructure.Endpoints;
using Microsoft.IdentityModel.JsonWebTokens;

namespace ClinicFlow.Api.Features.Auth.Me;

public sealed record MeResponse(string? Id, string? Email, string? FullName, DateTimeOffset? ExpiresAtUtc);

public sealed class MeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // No database call: everything comes from the claims inside the validated token.
        app.MapAuthGroup().MapGet("/me", (ClaimsPrincipal user) =>
        {
            var exp = user.FindFirstValue(JwtRegisteredClaimNames.Exp);

            return TypedResults.Ok(new MeResponse(
                user.FindFirstValue(JwtRegisteredClaimNames.Sub),
                user.FindFirstValue(JwtRegisteredClaimNames.Email),
                user.FindFirstValue(JwtRegisteredClaimNames.Name),
                exp is null ? null : DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp))));
        })
        .RequireAuthorization()
        .WithName("Me")
        .WithSummary("Who am I? Reads the claims from the token");
    }
}
