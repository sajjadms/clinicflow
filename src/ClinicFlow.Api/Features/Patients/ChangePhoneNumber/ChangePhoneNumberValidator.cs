using FluentValidation;

namespace ClinicFlow.Api.Features.Patients.ChangePhoneNumber;

public sealed class ChangePhoneNumberValidator : AbstractValidator<ChangePhoneNumberRequest>
{
    public ChangePhoneNumberValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[0-9]{7,15}$")
            .WithMessage("Phone number must be 7 to 15 digits, optionally starting with +.");
    }
}
