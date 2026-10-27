using ClinicFlow.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Features.Patients.UpdatePatient;

// Body of PUT /api/patients/{id}; the id comes from the route.
public sealed record UpdatePatientRequest(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email,
    string PhoneNumber)
{
    public UpdatePatientCommand ToCommand(Guid id) =>
        new(id, FirstName, LastName, DateOfBirth, Email, PhoneNumber);
}

public sealed record UpdatePatientCommand(
    Guid Id,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email,
    string PhoneNumber) : IRequest<PatientResponse?>;

public sealed class UpdatePatientHandler(ClinicFlowDbContext db)
    : IRequestHandler<UpdatePatientCommand, PatientResponse?>
{
    public async Task<PatientResponse?> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await db.Patients
            .IncludingInactive()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (patient is null)
        {
            return null;
        }

        patient.FirstName = request.FirstName.Trim();
        patient.LastName = request.LastName.Trim();
        patient.DateOfBirth = request.DateOfBirth;
        patient.Email = request.Email.Trim().ToLowerInvariant();
        patient.PhoneNumber = request.PhoneNumber.Trim();
        patient.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        return patient.ToResponse();
    }
}
