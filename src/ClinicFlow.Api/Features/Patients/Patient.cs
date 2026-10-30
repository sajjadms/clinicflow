using ClinicFlow.Api.Features.Nationalities;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Features.Patients;

public sealed class Patient
{
    public Guid Id { get; init; }

    public string? FileNo { get; set; }

    // Name parts
    public string FirstName { get; set; } = "";

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = "";

    // Demographics
    public DateOnly DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public MaritalStatus MaritalStatus { get; set; }

    // Optional: not every registration desk knows it on day one
    public int? NationalityId { get; set; }

    public Nationality? Nationality { get; set; }

    // Identity: unique per type (a passport number and a national ID can look the same)
    public IdentityType IdentityType { get; set; }

    public string IdentityNumber { get; set; } = "";

    // Contact
    public string MobileNumber { get; set; } = "";

    public string? Email { get; set; }

    public Address Address { get; set; } = null!;

    // Guardian is optional; the validator requires it for patients under 18
    public Guardian? Guardian { get; set; }

    // Social info (all optional)
    public string? Occupation { get; set; }

    public EducationLevel? EducationLevel { get; set; }

    public Religion? Religion { get; set; }

    // Status and audit
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; init; }

    public DateTime? UpdatedAtUtc { get; set; }

    public DateTime? DeactivatedAtUtc { get; set; }

    public string? DeactivatedBy { get; set; }
}

/// <summary>Complex type: stored as columns of the Patients table, no extra table or join.</summary>
public sealed class Address
{
    public required string Line1 { get; set; }

    public string? Line2 { get; set; }

    public required string City { get; set; }

    public string? Region { get; set; }

    public string? PostalCode { get; set; }
}

/// <summary>Optional complex type (EF Core 10): all its columns are null when there is no guardian.</summary>
public sealed class Guardian
{
    public required string FullName { get; set; }

    public GuardianRelationship Relationship { get; set; }

    public required string MobileNumber { get; set; }
}

public static class PatientFilters
{
    // Name of the EF Core query filter that hides deactivated patients.
    public const string ActiveOnly = "ActiveOnly";

    public static IQueryable<Patient> IncludingInactive(this IQueryable<Patient> patients) =>
        patients.IgnoreQueryFilters([ActiveOnly]);
}
