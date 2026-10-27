using ClinicFlow.Api.Infrastructure.Endpoints;
using ClinicFlow.Api.Infrastructure.Validation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ClinicFlow.Api.Features.Patients.ChangePhoneNumber;

public sealed class ChangePhoneNumberEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // PATCH changes part of a resource; PUT (UpdatePatient) replaces all of it.
        app.MapPatientsGroup().MapPatch("{id:guid}/phone-number", async Task<Results<Ok<PatientResponse>, NotFound>> (
            Guid id,
            ChangePhoneNumberRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var patient = await sender.Send(new ChangePhoneNumberCommand(id, request.PhoneNumber), ct);

            return patient is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(patient);
        })
        .WithName("ChangePhoneNumber")
        .WithSummary("Change only the patient's phone number")
        .WithValidation<ChangePhoneNumberRequest>();
    }
}
