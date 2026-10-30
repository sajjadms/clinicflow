using ClinicFlow.Api.Infrastructure.Persistence;
using FluentValidation;

namespace ClinicFlow.Api.Features.Patients.CreatePatient;

public sealed class CreatePatientValidator : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientValidator(ClinicFlowDbContext db)
    {
        Include(new PatientDetailsValidator(db));
    }
}
