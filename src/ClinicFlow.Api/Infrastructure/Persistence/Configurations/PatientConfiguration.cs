using ClinicFlow.Api.Features.Patients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Api.Infrastructure.Persistence.Configurations;

public sealed class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FileNo).HasMaxLength(20);
        builder.Property(p => p.FirstName).HasMaxLength(100);
        builder.Property(p => p.MiddleName).HasMaxLength(100);
        builder.Property(p => p.LastName).HasMaxLength(100);
        builder.Property(p => p.IdentityNumber).HasMaxLength(30);
        builder.Property(p => p.MobileNumber).HasMaxLength(16);
        builder.Property(p => p.Email).HasMaxLength(256);
        builder.Property(p => p.Occupation).HasMaxLength(100);
        builder.Property(p => p.DeactivatedBy).HasMaxLength(100);

        // Foreign key to the Nationalities lookup table.
        builder.HasOne(p => p.Nationality)
            .WithMany()
            .HasForeignKey(p => p.NationalityId)
            .OnDelete(DeleteBehavior.Restrict);

        // Complex types: their properties become columns of the Patients table.
        builder.ComplexProperty(p => p.Address, address =>
        {
            address.Property(a => a.Line1).HasColumnName("AddressLine1").HasMaxLength(200);
            address.Property(a => a.Line2).HasColumnName("AddressLine2").HasMaxLength(200);
            address.Property(a => a.City).HasColumnName("City").HasMaxLength(100);
            address.Property(a => a.Region).HasColumnName("Region").HasMaxLength(100);
            address.Property(a => a.PostalCode).HasColumnName("PostalCode").HasMaxLength(20);
        });

        builder.ComplexProperty(p => p.Guardian, guardian =>
        {
            guardian.Property(g => g.FullName).HasColumnName("GuardianName").HasMaxLength(200);
            guardian.Property(g => g.Relationship).HasColumnName("GuardianRelationship");
            guardian.Property(g => g.MobileNumber).HasColumnName("GuardianMobileNumber").HasMaxLength(16);
        });

        // The same identity number can't be registered twice for the same identity type.
        builder.HasIndex(p => new { p.IdentityType, p.IdentityNumber }).IsUnique();

        // File numbers are unique once assigned (NULL until then).
        builder.HasIndex(p => p.FileNo).IsUnique().HasFilter("[FileNo] IS NOT NULL");

        // Supports the list endpoint's ORDER BY LastName, FirstName.
        builder.HasIndex(p => new { p.LastName, p.FirstName });

        builder.HasIndex(p => p.MobileNumber);

        // Named query filter (EF Core 10): every query skips deactivated patients
        // unless it opts out with IgnoreQueryFilters([PatientFilters.ActiveOnly]).
        builder.HasQueryFilter(PatientFilters.ActiveOnly, p => p.IsActive);
    }
}
