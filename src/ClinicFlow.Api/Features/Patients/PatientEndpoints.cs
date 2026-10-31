using ClinicFlow.Api.Infrastructure.Logging;

namespace ClinicFlow.Api.Features.Patients;

public static class PatientEndpoints
{
    /// <summary>
    /// The route group every patient endpoint starts from: shared prefix, Swagger tag and filters.
    /// RequireAuthorization() here protects every patient endpoint at once: no valid token, 401.
    /// </summary>
    public static RouteGroupBuilder MapPatientsGroup(this IEndpointRouteBuilder app) =>
        app.MapGroup("/api/patients")
           .WithTags("Patients")
           .RequireAuthorization()
           .AddEndpointFilter<RequestTimingFilter>();
}
