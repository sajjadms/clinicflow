using ClinicFlow.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Features.Patients.CreatePatient;

public sealed record CreatePatientCommand(
    string FirstName,
    string? MiddleName,
    string LastName,
    DateOnly DateOfBirth,
    Gender Gender,
    MaritalStatus MaritalStatus,
    string? NationalityCode,
    IdentityType IdentityType,
    string IdentityNumber,
    string MobileNumber,
    string? Email,
    AddressDto Address,
    GuardianDto? Guardian,
    string? Occupation,
    EducationLevel? EducationLevel,
    Religion? Religion) : IPatientDetails, IRequest<PatientResponse>;

public sealed class CreatePatientHandler(ClinicFlowDbContext db, ILogger<CreatePatientHandler> logger)
    : IRequestHandler<CreatePatientCommand, PatientResponse>
{
    public async Task<PatientResponse> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var nationalityId = await ResolveNationalityIdAsync(request.NationalityCode, cancellationToken);

        var patient = new Patient { Id = Guid.NewGuid(), CreatedAtUtc = DateTime.UtcNow };
        patient.Apply(request, nationalityId);

        db.Patients.Add(patient);
        await db.SaveChangesAsync(cancellationToken);

        // Structured log: PatientId and Nationality become searchable properties, not just text.
        // No names, mobile or identity numbers in logs: they are personal data.
        logger.LogInformation("Patient {PatientId} registered, nationality {Nationality}",
            patient.Id, request.NationalityCode ?? "none");

        return (await db.GetPatientResponseAsync(patient.Id, cancellationToken))!;
    }

    // The validator already checked the code exists; here we turn it into the foreign key (or null).
    private async Task<int?> ResolveNationalityIdAsync(string? code, CancellationToken ct) =>
        string.IsNullOrWhiteSpace(code)
            ? null
            : await db.Nationalities.Where(n => n.Code == code.ToUpper()).Select(n => (int?)n.Id).SingleAsync(ct);
}
