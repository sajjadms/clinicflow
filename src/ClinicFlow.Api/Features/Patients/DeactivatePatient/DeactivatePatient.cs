using ClinicFlow.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Features.Patients.DeactivatePatient;

// Soft delete: the record stays for history. Returns false when the patient does not exist.
public sealed record DeactivatePatientCommand(Guid Id, string? DeactivatedBy) : IRequest<bool>;

public sealed class DeactivatePatientHandler(ClinicFlowDbContext db)
    : IRequestHandler<DeactivatePatientCommand, bool>
{
    public async Task<bool> Handle(DeactivatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await db.Patients
            .IncludingInactive()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (patient is null)
        {
            return false;
        }

        // Deactivating twice is harmless: the first timestamp is kept.
        if (patient.IsActive)
        {
            patient.IsActive = false;
            patient.DeactivatedAtUtc = DateTime.UtcNow;
            patient.DeactivatedBy = request.DeactivatedBy?.Trim();
            patient.UpdatedAtUtc = DateTime.UtcNow;

            await db.SaveChangesAsync(cancellationToken);
        }

        return true;
    }
}
