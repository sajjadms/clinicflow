using ClinicFlow.Api.Infrastructure.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ClinicFlow.Api.Features.Patients.DeletePatient;

public sealed class DeletePatientEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatientsGroup().MapDelete("{id:guid}", async Task<Results<NoContent, NotFound>> (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var deleted = await sender.Send(new DeletePatientCommand(id), ct);

            return deleted
                ? TypedResults.NoContent()
                : TypedResults.NotFound();
        })
        .WithName("DeletePatient")
        .WithSummary("Delete a patient permanently");
    }
}
