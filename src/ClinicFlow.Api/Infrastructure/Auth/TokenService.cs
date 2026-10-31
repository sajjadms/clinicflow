using System.Security.Claims;
using System.Text;
using ClinicFlow.Api.Features.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ClinicFlow.Api.Infrastructure.Auth;

public sealed record AccessToken(string Value, DateTime ExpiresAtUtc);

/// <summary>Creates the signed JWT that login returns.</summary>
public sealed class TokenService(IOptions<JwtOptions> options)
{
    public AccessToken CreateToken(User user)
    {
        var jwt = options.Value;
        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(jwt.ExpiryMinutes);

        // Payload: who the user is. Readable by anyone, so no secrets in here.
        var claims = new ClaimsIdentity(
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ]);

        // Signature: HMAC-SHA256 of header + payload with our secret key.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = jwt.Issuer,
            Audience = jwt.Audience,
            Subject = claims,
            IssuedAt = now,
            NotBefore = now,
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        return new AccessToken(new JsonWebTokenHandler().CreateToken(descriptor), expiresAt);
    }
}
