using ClinicFlow.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Features.Patients.ChangePhoneNumber;

// Body of PATCH /api/patients/{id}/phone-number: only the field that changes.
public sealed record ChangePhoneNumberRequest(string PhoneNumber);

public sealed record ChangePhoneNumberCommand(Guid Id, string PhoneNumber) : IRequest<PatientResponse?>;

public sealed class ChangePhoneNumberHandler(ClinicFlowDbContext db)
    : IRequestHandler<ChangePhoneNumberCommand, PatientResponse?>
{
    public async Task<PatientResponse?> Handle(ChangePhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var patient = await db.Patients
            .IncludingInactive()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (patient is null)
        {
            return null;
        }

        patient.PhoneNumber = request.PhoneNumber.Trim();
        patient.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        return patient.ToResponse();
    }
}
