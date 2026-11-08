using ClinicFlow.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Features.Patients.FileNumbers;

/// <summary>Works out the next clinic file number, e.g. CF-2026-000001.</summary>
public interface IFileNoGenerator
{
    Task<string> NextAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Transient: small, stateless, cheap to create, so every consumer gets its own instance.
/// It may depend on the scoped DbContext because a transient lives no longer than its consumer.
/// </summary>
public sealed class FileNoGenerator(ClinicFlowDbContext db, TimeProvider clock) : IFileNoGenerator
{
    public async Task<string> NextAsync(CancellationToken cancellationToken)
    {
        // File numbers restart every year. The unique index on FileNo stops duplicates.
        var prefix = $"CF-{clock.GetUtcNow().Year}-";

        var lastFileNo = await db.Patients
            .IncludingInactive()
            .Where(p => p.FileNo != null && p.FileNo.StartsWith(prefix))
            .OrderByDescending(p => p.FileNo)
            .Select(p => p.FileNo)
            .FirstOrDefaultAsync(cancellationToken);

        var next = lastFileNo is null ? 1 : int.Parse(lastFileNo[prefix.Length..]) + 1;

        return $"{prefix}{next:D6}";
    }
}
