namespace ClinicFlow.Api.Features.Patients;

// Enums start at 1, so a missing value (0) fails validation instead of silently becoming a default.
// The API reads and writes them as names ("Female"), and SQL Server stores them as text.

public enum Gender
{
    Male = 1,
    Female = 2
}

public enum MaritalStatus
{
    Single = 1,
    Married = 2,
    Divorced = 3,
    Widowed = 4
}

public enum IdentityType
{
    NationalId = 1,
    Passport = 2,
    ResidenceId = 3
}

public enum GuardianRelationship
{
    Father = 1,
    Mother = 2,
    Spouse = 3,
    Sibling = 4,
    Child = 5,
    LegalGuardian = 6,
    Other = 7
}

public enum EducationLevel
{
    None = 1,
    Primary = 2,
    Secondary = 3,
    Diploma = 4,
    Bachelor = 5,
    Master = 6,
    Doctorate = 7
}

public enum Religion
{
    Islam = 1,
    Christianity = 2,
    Hinduism = 3,
    Buddhism = 4,
    Sikhism = 5,
    Judaism = 6,
    Other = 7,
    PreferNotToSay = 8
}
