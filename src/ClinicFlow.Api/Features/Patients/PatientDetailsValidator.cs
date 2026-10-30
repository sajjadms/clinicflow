using ClinicFlow.Api.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Features.Patients;

/// <summary>
/// One set of rules for every endpoint that receives full patient details.
/// Create and Update include it, so a rule changes in one place.
/// </summary>
public sealed class PatientDetailsValidator : AbstractValidator<IPatientDetails>
{
    // International format: + then 8 to 15 digits, for example +919876543210
    private const string MobilePattern = @"^\+[1-9][0-9]{7,14}$";

    public PatientDetailsValidator(ClinicFlowDbContext db)
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MiddleName).MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);

        RuleFor(x => x.DateOfBirth)
            .Must(dob => dob <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Date of birth cannot be in the future.")
            .Must(dob => dob >= new DateOnly(1900, 1, 1))
            .WithMessage("Date of birth must be after 1900-01-01.");

        RuleFor(x => x.Gender).IsInEnum();
        RuleFor(x => x.MaritalStatus).IsInEnum();
        RuleFor(x => x.IdentityType).IsInEnum();

        RuleFor(x => x.IdentityNumber)
            .NotEmpty()
            .MaximumLength(30)
            .Matches("^[A-Za-z0-9-]+$")
            .WithMessage("Identity number can only contain letters, digits and '-'.");

        // Optional, but when sent it must exist in the Nationalities table (ISO 3166-1 alpha-2, for example "IN").
        RuleFor(x => x.NationalityCode)
            .Length(2)
            .MustAsync((code, ct) => db.Nationalities.AnyAsync(n => n.Code == code!.ToUpper(), ct))
            .WithMessage("Unknown nationality code. Use GET /api/nationalities for the list.")
            .When(x => !string.IsNullOrWhiteSpace(x.NationalityCode));

        RuleFor(x => x.MobileNumber)
            .NotEmpty()
            .Matches(MobilePattern)
            .WithMessage("Mobile number must be in international format, for example +919876543210.");

        RuleFor(x => x.Email).EmailAddress().MaximumLength(256).When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Address).NotNull();
        RuleFor(x => x.Address.Line1).NotEmpty().MaximumLength(200).When(x => x.Address is not null);
        RuleFor(x => x.Address.Line2).MaximumLength(200).When(x => x.Address is not null);
        RuleFor(x => x.Address.City).NotEmpty().MaximumLength(100).When(x => x.Address is not null);
        RuleFor(x => x.Address.Region).MaximumLength(100).When(x => x.Address is not null);
        RuleFor(x => x.Address.PostalCode).MaximumLength(20).When(x => x.Address is not null);

        // Patients under 18 must have a guardian.
        RuleFor(x => x.Guardian)
            .NotNull()
            .When(x => x.DateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-18))
            .WithMessage("A guardian is required for patients under 18.");

        When(x => x.Guardian is not null, () =>
        {
            RuleFor(x => x.Guardian!.FullName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Guardian!.Relationship).IsInEnum();
            RuleFor(x => x.Guardian!.MobileNumber)
                .NotEmpty()
                .Matches(MobilePattern)
                .WithMessage("Guardian mobile number must be in international format.");
        });

        RuleFor(x => x.Occupation).MaximumLength(100);
        RuleFor(x => x.EducationLevel).IsInEnum().When(x => x.EducationLevel is not null);
        RuleFor(x => x.Religion).IsInEnum().When(x => x.Religion is not null);
    }
}
