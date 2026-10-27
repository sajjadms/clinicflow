using ClinicFlow.Api.Features.Patients;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Infrastructure.Persistence;

public sealed class ClinicFlowDbContext(DbContextOptions<ClinicFlowDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Named query filter (EF Core 10): every query skips deactivated patients
        // unless it opts out with IgnoreQueryFilters([PatientFilters.ActiveOnly]).
        modelBuilder.Entity<Patient>()
            .HasQueryFilter(PatientFilters.ActiveOnly, p => p.IsActive);
    }
}
