using ClinicFlow.Api.Features.Nationalities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Api.Infrastructure.Persistence.Configurations;

public sealed class NationalityConfiguration : IEntityTypeConfiguration<Nationality>
{
    public void Configure(EntityTypeBuilder<Nationality> builder)
    {
        builder.ToTable("Nationalities");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).ValueGeneratedNever();   // ISO numeric code, not an identity column

        builder.Property(n => n.Code).HasMaxLength(2).IsFixedLength().IsUnicode(false);
        builder.Property(n => n.Name).HasMaxLength(100);
        builder.HasIndex(n => n.Code).IsUnique();

        // Seed data: the first migration inserts all ISO 3166-1 countries.
        builder.HasData(NationalitySeed.All);
    }
}
