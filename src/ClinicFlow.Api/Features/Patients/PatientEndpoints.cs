using ClinicFlow.Api.Features.Patients.CreatePatient;
using ClinicFlow.Api.Features.Patients.GetPatientById;
using ClinicFlow.Api.Features.Patients.ListPatients;
using ClinicFlow.Api.Infrastructure.Validation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ClinicFlow.Api.Features.Patients;

public static class PatientEndpoints
{
    public static IEndpointRouteBuilder MapPatientEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/patients")
                       .WithTags("Patients");

        group.MapPost("", async (CreatePatientCommand command, ISender sender, CancellationToken ct) =>
        {
            var patient = await sender.Send(command, ct);

            return TypedResults.CreatedAtRoute(patient, "GetPatientById", new { id = patient.Id });
        })
        .WithName("CreatePatient")
        .WithSummary("Register a new patient")
        .WithValidation<CreatePatientCommand>();

        group.MapGet("{id:guid}", async Task<Results<Ok<PatientResponse>, NotFound>> (Guid id, ISender sender, CancellationToken ct) =>
        {
            var patient = await sender.Send(new GetPatientByIdQuery(id), ct);

            return patient is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(patient);
        })
        .WithName("GetPatientById")
        .WithSummary("Get a patient by id");

        group.MapGet("", async (ISender sender, CancellationToken ct, int page = 1, int pageSize = 20) =>
        {
            var patients = await sender.Send(new ListPatientsQuery(page, pageSize), ct);

            return TypedResults.Ok(patients);
        })
        .WithName("ListPatients")
        .WithSummary("List patients, sorted by name");

        return app;
    }
}
