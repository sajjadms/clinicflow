using ClinicFlow.Api.Infrastructure.Auth;
using ClinicFlow.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Features.Patients.DeactivatePatient;

// Soft delete: the record stays for history. Returns false when the patient does not exist.
public sealed record DeactivatePatientCommand(Guid Id) : IRequest<bool>;

public sealed class DeactivatePatientHandler(ClinicFlowDbContext db, ICurrentUser currentUser, TimeProvider clock)
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
            var now = clock.GetUtcNow().UtcDateTime;
            patient.DeactivatedAtUtc = now;
            patient.DeactivatedBy = currentUser.Email;   // from the signed-in user's token, not from the client
            patient.UpdatedAtUtc = now;

            await db.SaveChangesAsync(cancellationToken);
        }

        return true;
    }
}
