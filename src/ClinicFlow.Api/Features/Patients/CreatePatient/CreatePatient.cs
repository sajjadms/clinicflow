using ClinicFlow.Api.Infrastructure.Persistence;
using MediatR;

namespace ClinicFlow.Api.Features.Patients.CreatePatient;

public sealed record CreatePatientCommand(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email,
    string PhoneNumber) : IRequest<PatientResponse>;

public sealed class CreatePatientHandler(ClinicFlowDbContext db)
    : IRequestHandler<CreatePatientCommand, PatientResponse>
{
    public async Task<PatientResponse> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            DateOfBirth = request.DateOfBirth,
            Email = request.Email.Trim().ToLowerInvariant(),
            PhoneNumber = request.PhoneNumber.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        db.Patients.Add(patient);
        await db.SaveChangesAsync(cancellationToken);

        return patient.ToResponse();
    }
}
