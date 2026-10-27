using ClinicFlow.Api.Infrastructure.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Features.Patients.DeactivatePatient;

public sealed class DeactivatePatientEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // Who deactivated the patient comes from a header for now; after the JWT video it comes from the signed-in user.
        app.MapPatientsGroup().MapPost("{id:guid}/deactivate", async Task<Results<NoContent, NotFound>> (
            Guid id,
            [FromHeader(Name = "X-Requested-By")] string? requestedBy,
            ISender sender,
            CancellationToken ct) =>
        {
            var found = await sender.Send(new DeactivatePatientCommand(id, requestedBy), ct);

            return found
                ? TypedResults.NoContent()
                : TypedResults.NotFound();
        })
        .WithName("DeactivatePatient")
        .WithSummary("Deactivate a patient (soft delete)");
    }
}
