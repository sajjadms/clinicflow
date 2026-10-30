using ClinicFlow.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Features.Patients.ChangeMobileNumber;

// Body of PATCH /api/patients/{id}/mobile-number: only the field that changes.
public sealed record ChangeMobileNumberRequest(string MobileNumber);

public sealed record ChangeMobileNumberCommand(Guid Id, string MobileNumber) : IRequest<PatientResponse?>;

public sealed class ChangeMobileNumberHandler(ClinicFlowDbContext db)
    : IRequestHandler<ChangeMobileNumberCommand, PatientResponse?>
{
    public async Task<PatientResponse?> Handle(ChangeMobileNumberCommand request, CancellationToken cancellationToken)
    {
        var patient = await db.Patients
            .IncludingInactive()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (patient is null)
        {
            return null;
        }

        patient.MobileNumber = request.MobileNumber.Trim();
        patient.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        return await db.GetPatientResponseAsync(patient.Id, cancellationToken);
    }
}
