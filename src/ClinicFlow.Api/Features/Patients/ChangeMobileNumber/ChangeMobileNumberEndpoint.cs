using ClinicFlow.Api.Infrastructure.Endpoints;
using ClinicFlow.Api.Infrastructure.Validation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ClinicFlow.Api.Features.Patients.ChangeMobileNumber;

public sealed class ChangeMobileNumberEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // PATCH changes part of a resource; PUT (UpdatePatient) replaces all of it.
        app.MapPatientsGroup().MapPatch("{id:guid}/mobile-number", async Task<Results<Ok<PatientResponse>, NotFound>> (
            Guid id,
            ChangeMobileNumberRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var patient = await sender.Send(new ChangeMobileNumberCommand(id, request.MobileNumber), ct);

            return patient is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(patient);
        })
        .WithName("ChangeMobileNumber")
        .WithSummary("Change only the patient's mobile number")
        .WithValidation<ChangeMobileNumberRequest>();
    }
}
