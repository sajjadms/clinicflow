using ClinicFlow.Api.Features.Auth;
using ClinicFlow.Api.Features.Nationalities;
using ClinicFlow.Api.Features.Patients;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Infrastructure.Persistence;

public sealed class ClinicFlowDbContext(DbContextOptions<ClinicFlowDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();

    public DbSet<Nationality> Nationalities => Set<Nationality>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Picks up every IEntityTypeConfiguration<T> in this project (Configurations folder).
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClinicFlowDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Store enums as readable text ("Female"), not numbers, in every table.
        configurationBuilder.Properties<Gender>().HaveConversion<string>().HaveMaxLength(20);
        configurationBuilder.Properties<MaritalStatus>().HaveConversion<string>().HaveMaxLength(20);
        configurationBuilder.Properties<IdentityType>().HaveConversion<string>().HaveMaxLength(20);
        configurationBuilder.Properties<GuardianRelationship>().HaveConversion<string>().HaveMaxLength(20);
        configurationBuilder.Properties<EducationLevel>().HaveConversion<string>().HaveMaxLength(20);
        configurationBuilder.Properties<Religion>().HaveConversion<string>().HaveMaxLength(20);
    }
}
