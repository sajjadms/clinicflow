using ClinicFlow.Api.Infrastructure.Auth;
using ClinicFlow.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Features.Auth.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResponse?>;

public sealed record LoginResponse(string AccessToken, string TokenType, DateTime ExpiresAtUtc);

public sealed class LoginHandler(ClinicFlowDbContext db, IPasswordHasher<User> passwordHasher, TokenService tokenService)
    : IRequestHandler<LoginCommand, LoginResponse?>
{
    public async Task<LoginResponse?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);

        // Unknown email or wrong password: the same answer (null -> 401), so nobody can probe which emails exist.
        if (user is null)
        {
            return null;
        }

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        var token = tokenService.CreateToken(user);

        return new LoginResponse(token.Value, "Bearer", token.ExpiresAtUtc);
    }
}
