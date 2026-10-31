using ClinicFlow.Api.Infrastructure.Endpoints;
using ClinicFlow.Api.Infrastructure.Validation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ClinicFlow.Api.Features.Auth.Register;

public sealed class RegisterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapAuthGroup().MapPost("/register", async Task<Results<Created<UserResponse>, ProblemHttpResult>> (
            RegisterCommand command,
            ISender sender,
            CancellationToken ct) =>
        {
            var user = await sender.Send(command, ct);

            return user is null
                ? TypedResults.Problem(statusCode: StatusCodes.Status409Conflict, title: "This email is already registered.")
                : TypedResults.Created((string?)null, user);
        })
        .WithName("Register")
        .WithSummary("Create a user account")
        .WithValidation<RegisterCommand>();
    }
}
