namespace ClinicFlow.Api.Features.Patients;

public sealed record PatientResponse(
    Guid Id,
    string? FileNo,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email,
    string PhoneNumber,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public static class PatientMappings
{
    public static PatientResponse ToResponse(this Patient patient) => new(
        patient.Id,
        patient.FileNo,
        patient.FirstName,
        patient.LastName,
        patient.DateOfBirth,
        patient.Email,
        patient.PhoneNumber,
        patient.IsActive,
        patient.CreatedAtUtc,
        patient.UpdatedAtUtc);
}
