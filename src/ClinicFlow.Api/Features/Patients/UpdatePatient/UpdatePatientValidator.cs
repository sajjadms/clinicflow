using ClinicFlow.Api.Infrastructure.Persistence;
using FluentValidation;

namespace ClinicFlow.Api.Features.Patients.UpdatePatient;

public sealed class UpdatePatientValidator : AbstractValidator<UpdatePatientRequest>
{
    public UpdatePatientValidator(ClinicFlowDbContext db)
    {
        Include(new PatientDetailsValidator(db));
    }
}
