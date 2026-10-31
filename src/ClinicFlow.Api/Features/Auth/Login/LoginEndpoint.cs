using ClinicFlow.Api.Infrastructure.Endpoints;
using ClinicFlow.Api.Infrastructure.Validation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ClinicFlow.Api.Features.Auth.Login;

public sealed class LoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapAuthGroup().MapPost("/login", async Task<Results<Ok<LoginResponse>, ProblemHttpResult>> (
            LoginCommand command,
            ISender sender,
            CancellationToken ct) =>
        {
            var response = await sender.Send(command, ct);

            return response is null
                ? TypedResults.Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Invalid email or password.")
                : TypedResults.Ok(response);
        })
        .WithName("Login")
        .WithSummary("Sign in and get a JWT access token")
        .WithValidation<LoginCommand>();
    }
}
