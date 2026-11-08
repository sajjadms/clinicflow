using ClinicFlow.Api.Features.Patients.FileNumbers;
using ClinicFlow.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Features.Patients.GenerateFileNo;

public sealed record GenerateFileNoCommand(Guid Id) : IRequest<GenerateFileNoResult>;

public enum GenerateFileNoStatus
{
    Generated,
    NotFound,
    AlreadyAssigned
}

public sealed record GenerateFileNoResult(GenerateFileNoStatus Status, PatientResponse? Patient = null);

public sealed class GenerateFileNoHandler(ClinicFlowDbContext db, IFileNoGenerator fileNos, TimeProvider clock)
    : IRequestHandler<GenerateFileNoCommand, GenerateFileNoResult>
{
    public async Task<GenerateFileNoResult> Handle(GenerateFileNoCommand request, CancellationToken cancellationToken)
    {
        var patient = await db.Patients
            .IncludingInactive()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (patient is null)
        {
            return new(GenerateFileNoStatus.NotFound);
        }

        if (patient.FileNo is not null)
        {
            return new(GenerateFileNoStatus.AlreadyAssigned, await db.GetPatientResponseAsync(patient.Id, cancellationToken));
        }

        patient.FileNo = await fileNos.NextAsync(cancellationToken);
        patient.UpdatedAtUtc = clock.GetUtcNow().UtcDateTime;

        await db.SaveChangesAsync(cancellationToken);

        return new(GenerateFileNoStatus.Generated, await db.GetPatientResponseAsync(patient.Id, cancellationToken));
    }
}
