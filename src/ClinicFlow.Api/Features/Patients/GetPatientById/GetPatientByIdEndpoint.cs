using ClinicFlow.Api.Infrastructure.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ClinicFlow.Api.Features.Patients.GetPatientById;

public sealed class GetPatientByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatientsGroup().MapGet("{id:guid}", async Task<Results<Ok<PatientResponse>, NotFound>> (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var patient = await sender.Send(new GetPatientByIdQuery(id), ct);

            return patient is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(patient);
        })
        .WithName("GetPatientById")
        .WithSummary("Get a patient by id");
    }
}
