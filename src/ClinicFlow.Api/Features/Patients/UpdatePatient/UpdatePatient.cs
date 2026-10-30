using ClinicFlow.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Features.Patients.UpdatePatient;

// Body of PUT /api/patients/{id}; the id comes from the route. PUT replaces every field.
public sealed record UpdatePatientRequest(
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
    Religion? Religion) : IPatientDetails
{
    public UpdatePatientCommand ToCommand(Guid id) => new(id, this);
}

public sealed record UpdatePatientCommand(Guid Id, IPatientDetails Details) : IRequest<PatientResponse?>;

public sealed class UpdatePatientHandler(ClinicFlowDbContext db)
    : IRequestHandler<UpdatePatientCommand, PatientResponse?>
{
    public async Task<PatientResponse?> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        // Load: IncludingInactive means we can update a deactivated patient too.
        var patient = await db.Patients
            .IncludingInactive()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (patient is null)
        {
            return null;
        }

        var nationalityId = await ResolveNationalityIdAsync(request.Details.NationalityCode, cancellationToken);

        // Update every field, then save.
        patient.Apply(request.Details, nationalityId);
        patient.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        return await db.GetPatientResponseAsync(patient.Id, cancellationToken);
    }

    // The validator already checked the code exists; here we turn it into the foreign key (or null).
    private async Task<int?> ResolveNationalityIdAsync(string? code, CancellationToken ct) =>
        string.IsNullOrWhiteSpace(code)
            ? null
            : await db.Nationalities.Where(n => n.Code == code.ToUpper()).Select(n => (int?)n.Id).SingleAsync(ct);
}
