using System.Linq.Expressions;
using ClinicFlow.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Features.Patients;

public sealed record NationalityDto(string Code, string Name);

public sealed record PatientResponse(
    Guid Id,
    string? FileNo,
    string FirstName,
    string? MiddleName,
    string LastName,
    DateOnly DateOfBirth,
    Gender Gender,
    MaritalStatus MaritalStatus,
    NationalityDto? Nationality,
    IdentityType IdentityType,
    string IdentityNumber,
    string MobileNumber,
    string? Email,
    AddressDto Address,
    GuardianDto? Guardian,
    string? Occupation,
    EducationLevel? EducationLevel,
    Religion? Religion,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public static class PatientProjections
{
    /// <summary>
    /// Used inside Select(...): EF Core turns it into one SQL query that LEFT JOINs Nationalities
    /// and reads only these columns. No Include and no tracking needed.
    /// </summary>
    public static readonly Expression<Func<Patient, PatientResponse>> ToResponse = p => new PatientResponse(
        p.Id,
        p.FileNo,
        p.FirstName,
        p.MiddleName,
        p.LastName,
        p.DateOfBirth,
        p.Gender,
        p.MaritalStatus,
        p.Nationality == null ? null : new NationalityDto(p.Nationality.Code, p.Nationality.Name),
        p.IdentityType,
        p.IdentityNumber,
        p.MobileNumber,
        p.Email,
        new AddressDto(p.Address.Line1, p.Address.Line2, p.Address.City, p.Address.Region, p.Address.PostalCode),
        p.Guardian == null ? null : new GuardianDto(p.Guardian.FullName, p.Guardian.Relationship, p.Guardian.MobileNumber),
        p.Occupation,
        p.EducationLevel,
        p.Religion,
        p.IsActive,
        p.CreatedAtUtc,
        p.UpdatedAtUtc);

    /// <summary>Reads one patient (active or not) in the response shape, or null.</summary>
    public static Task<PatientResponse?> GetPatientResponseAsync(this ClinicFlowDbContext db, Guid id, CancellationToken ct) =>
        db.Patients
            .AsNoTracking()
            .IncludingInactive()
            .Where(p => p.Id == id)
            .Select(ToResponse)
            .FirstOrDefaultAsync(ct);
}
