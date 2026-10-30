using FluentValidation;

namespace ClinicFlow.Api.Features.Patients.ChangeMobileNumber;

public sealed class ChangeMobileNumberValidator : AbstractValidator<ChangeMobileNumberRequest>
{
    public ChangeMobileNumberValidator()
    {
        RuleFor(x => x.MobileNumber)
            .NotEmpty()
            .Matches(@"^\+[1-9][0-9]{7,14}$")
            .WithMessage("Mobile number must be in international format, for example +919876543210.");
    }
}
