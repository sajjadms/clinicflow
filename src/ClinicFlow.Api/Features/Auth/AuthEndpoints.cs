namespace ClinicFlow.Api.Features.Auth;

public static class AuthEndpoints
{
    /// <summary>Route group for sign-up, sign-in and "who am I".</summary>
    public static RouteGroupBuilder MapAuthGroup(this IEndpointRouteBuilder app) =>
        app.MapGroup("/api/auth")
           .WithTags("Auth");
}
