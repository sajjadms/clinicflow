namespace ClinicFlow.Api.Features.Auth;

/// <summary>A person who can sign in to ClinicFlow (receptionist, doctor, admin).</summary>
public sealed class User
{
    public Guid Id { get; init; }

    public string Email { get; set; } = "";

    public string FullName { get; set; } = "";

    // Never the password itself: a salted, slow hash made by PasswordHasher.
    public string PasswordHash { get; set; } = "";

    public DateTime CreatedAtUtc { get; init; }
}
