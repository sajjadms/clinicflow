using ClinicFlow.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Features.Auth.Register;

public sealed record RegisterCommand(string Email, string FullName, string Password) : IRequest<UserResponse?>;

public sealed record UserResponse(Guid Id, string Email, string FullName);

public sealed class RegisterHandler(ClinicFlowDbContext db, IPasswordHasher<User> passwordHasher)
    : IRequestHandler<RegisterCommand, UserResponse?>
{
    public async Task<UserResponse?> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        // null tells the endpoint: this email is already taken (409).
        if (await db.Users.AnyAsync(u => u.Email == email, cancellationToken))
        {
            return null;
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FullName = request.FullName.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        // We store only the hash. PasswordHasher adds a random salt and runs PBKDF2 many times.
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        return new UserResponse(user.Id, user.Email, user.FullName);
    }
}
