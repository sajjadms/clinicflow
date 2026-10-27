using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Features.Patients;

public sealed class Patient
{
    public Guid Id { get; init; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public required string Email { get; set; }

    public required string PhoneNumber { get; set; }

    public string? FileNo { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; init; }

    public DateTime? UpdatedAtUtc { get; set; }

    public DateTime? DeactivatedAtUtc { get; set; }

    public string? DeactivatedBy { get; set; }
}

public static class PatientFilters
{
    // Name of the EF Core query filter that hides deactivated patients.
    public const string ActiveOnly = "ActiveOnly";

    public static IQueryable<Patient> IncludingInactive(this IQueryable<Patient> patients) =>
        patients.IgnoreQueryFilters([ActiveOnly]);
}
