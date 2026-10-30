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

public sealed class GenerateFileNoHandler(ClinicFlowDbContext db)
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

        // File numbers look like CF-2026-000001 and restart every year.
        // The unique index on FileNo stops duplicates; a database sequence comes in the EF Core video.
        var prefix = $"CF-{DateTime.UtcNow.Year}-";

        var lastFileNo = await db.Patients
            .IncludingInactive()
            .Where(p => p.FileNo != null && p.FileNo.StartsWith(prefix))
            .OrderByDescending(p => p.FileNo)
            .Select(p => p.FileNo)
            .FirstOrDefaultAsync(cancellationToken);

        var next = lastFileNo is null ? 1 : int.Parse(lastFileNo[prefix.Length..]) + 1;

        patient.FileNo = $"{prefix}{next:D6}";
        patient.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        return new(GenerateFileNoStatus.Generated, await db.GetPatientResponseAsync(patient.Id, cancellationToken));
    }
}
