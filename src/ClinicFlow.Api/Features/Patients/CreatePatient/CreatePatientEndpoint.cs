using ClinicFlow.Api.Infrastructure.Endpoints;
using ClinicFlow.Api.Infrastructure.Validation;
using MediatR;

namespace ClinicFlow.Api.Features.Patients.CreatePatient;

public sealed class CreatePatientEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatientsGroup().MapPost("", async (
            CreatePatientCommand command,
            ISender sender,
            CancellationToken ct) =>
        {
            var patient = await sender.Send(command, ct);

            return TypedResults.CreatedAtRoute(patient, "GetPatientById", new { id = patient.Id });
        })
        .WithName("CreatePatient")
        .WithSummary("Register a new patient")
        .WithValidation<CreatePatientCommand>();
    }
}
