namespace ClinicFlow.Api.Features.Patients;

/// <summary>
/// The fields a client sends when it creates or fully updates a patient.
/// CreatePatientCommand and UpdatePatientRequest both implement it, so they share one validator and one mapping.
/// </summary>
public interface IPatientDetails
{
    string FirstName { get; }
    string? MiddleName { get; }
    string LastName { get; }
    DateOnly DateOfBirth { get; }
    Gender Gender { get; }
    MaritalStatus MaritalStatus { get; }
    string? NationalityCode { get; }
    IdentityType IdentityType { get; }
    string IdentityNumber { get; }
    string MobileNumber { get; }
    string? Email { get; }
    AddressDto Address { get; }
    GuardianDto? Guardian { get; }
    string? Occupation { get; }
    EducationLevel? EducationLevel { get; }
    Religion? Religion { get; }
}

public sealed record AddressDto(string Line1, string? Line2, string City, string? Region, string? PostalCode);

public sealed record GuardianDto(string FullName, GuardianRelationship Relationship, string MobileNumber);

public static class PatientDetailsMapping
{
    /// <summary>Copies the client's details onto the entity, trimming text and normalising email.</summary>
    public static void Apply(this Patient patient, IPatientDetails details, int? nationalityId)
    {
        patient.FirstName = details.FirstName.Trim();
        patient.MiddleName = string.IsNullOrWhiteSpace(details.MiddleName) ? null : details.MiddleName.Trim();
        patient.LastName = details.LastName.Trim();
        patient.DateOfBirth = details.DateOfBirth;
        patient.Gender = details.Gender;
        patient.MaritalStatus = details.MaritalStatus;
        patient.NationalityId = nationalityId;
        patient.IdentityType = details.IdentityType;
        patient.IdentityNumber = details.IdentityNumber.Trim().ToUpperInvariant();
        patient.MobileNumber = details.MobileNumber.Trim();
        patient.Email = string.IsNullOrWhiteSpace(details.Email) ? null : details.Email.Trim().ToLowerInvariant();
        patient.Address = new Address
        {
            Line1 = details.Address.Line1.Trim(),
            Line2 = details.Address.Line2?.Trim(),
            City = details.Address.City.Trim(),
            Region = details.Address.Region?.Trim(),
            PostalCode = details.Address.PostalCode?.Trim()
        };
        patient.Guardian = details.Guardian is null
            ? null
            : new Guardian
            {
                FullName = details.Guardian.FullName.Trim(),
                Relationship = details.Guardian.Relationship,
                MobileNumber = details.Guardian.MobileNumber.Trim()
            };
        patient.Occupation = string.IsNullOrWhiteSpace(details.Occupation) ? null : details.Occupation.Trim();
        patient.EducationLevel = details.EducationLevel;
        patient.Religion = details.Religion;
    }
}
