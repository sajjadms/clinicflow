using ClinicFlow.Api.Infrastructure.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ClinicFlow.Api.Features.Patients.DeactivatePatient;

public sealed class DeactivatePatientEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // Who deactivated the patient comes from the signed-in user (ICurrentUser), not from the request.
        app.MapPatientsGroup().MapPost("{id:guid}/deactivate", async Task<Results<NoContent, NotFound>> (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var found = await sender.Send(new DeactivatePatientCommand(id), ct);

            return found
                ? TypedResults.NoContent()
                : TypedResults.NotFound();
        })
        .WithName("DeactivatePatient")
        .WithSummary("Deactivate a patient (soft delete)");
    }
}
