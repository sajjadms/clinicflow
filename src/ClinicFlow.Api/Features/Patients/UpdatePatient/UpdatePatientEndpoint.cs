using ClinicFlow.Api.Infrastructure.Endpoints;
using ClinicFlow.Api.Infrastructure.Validation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ClinicFlow.Api.Features.Patients.UpdatePatient;

public sealed class UpdatePatientEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatientsGroup().MapPut("{id:guid}", async Task<Results<Ok<PatientResponse>, NotFound>> (
            Guid id,
            UpdatePatientRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var patient = await sender.Send(request.ToCommand(id), ct);

            return patient is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(patient);
        })
        .WithName("UpdatePatient")
        .WithSummary("Update a patient's details")
        .WithValidation<UpdatePatientRequest>();
    }
}
