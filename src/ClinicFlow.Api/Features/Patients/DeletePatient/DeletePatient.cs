using ClinicFlow.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Features.Patients.DeletePatient;

// Hard delete. Returns false when the patient does not exist.
// With SQL Server this becomes a single ExecuteDeleteAsync call (the in-memory provider does not support it).
public sealed record DeletePatientCommand(Guid Id) : IRequest<bool>;

public sealed class DeletePatientHandler(ClinicFlowDbContext db)
    : IRequestHandler<DeletePatientCommand, bool>
{
    public async Task<bool> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await db.Patients
            .IncludingInactive()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (patient is null)
        {
            return false;
        }

        db.Patients.Remove(patient);
        await db.SaveChangesAsync(cancellationToken);

        return true;
    }
}
