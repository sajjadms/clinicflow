using ClinicFlow.Api.Features.Patients;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Infrastructure.Persistence;

public sealed class ClinicFlowDbContext(DbContextOptions<ClinicFlowDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();
}
