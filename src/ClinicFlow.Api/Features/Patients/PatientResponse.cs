namespace ClinicFlow.Api.Features.Patients;

public sealed record PatientResponse(
    Guid Id,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email,
    string PhoneNumber,
    DateTime CreatedAtUtc);

public static class PatientMappings
{
    public static PatientResponse ToResponse(this Patient patient) => new(
        patient.Id,
        patient.FirstName,
        patient.LastName,
        patient.DateOfBirth,
        patient.Email,
        patient.PhoneNumber,
        patient.CreatedAtUtc);
}
