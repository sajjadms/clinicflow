using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace ClinicFlow.Api.Infrastructure.Auth;

/// <summary>The signed-in user of the current request, read from the JWT.</summary>
public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    Guid? UserId { get; }

    string? Email { get; }

    string? Name { get; }
}

/// <summary>
/// Scoped: one instance per HTTP request. Every handler in the same request sees the same user,
/// and the next request (maybe another user) gets a fresh instance.
/// </summary>
public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal? User => accessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public Guid? UserId => Guid.TryParse(User?.FindFirstValue(JwtRegisteredClaimNames.Sub), out var id) ? id : null;

    public string? Email => User?.FindFirstValue(JwtRegisteredClaimNames.Email);

    public string? Name => User?.FindFirstValue(JwtRegisteredClaimNames.Name);
}
