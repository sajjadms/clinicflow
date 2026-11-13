using ClinicFlow.Api.Infrastructure.Auth;
using ClinicFlow.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Features.Auth.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResponse?>;

public sealed record LoginResponse(string AccessToken, string TokenType, DateTime ExpiresAtUtc);

public sealed class LoginHandler(
    ClinicFlowDbContext db,
    IPasswordHasher<User> passwordHasher,
    TokenService tokenService,
    ILogger<LoginHandler> logger)
    : IRequestHandler<LoginCommand, LoginResponse?>
{
    public async Task<LoginResponse?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);

        // Unknown email or wrong password: the same answer (null -> 401), so nobody can probe which emails exist.
        if (user is null)
        {
            logger.LogWarning("Login failed: unknown email");
            return null;
        }

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            // Never log the password or the token.
            logger.LogWarning("Login failed: wrong password for user {UserId}", user.Id);
            return null;
        }

        var token = tokenService.CreateToken(user);
        logger.LogInformation("User {UserId} logged in", user.Id);

        return new LoginResponse(token.Value, "Bearer", token.ExpiresAtUtc);
    }
}
