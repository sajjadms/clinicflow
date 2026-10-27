using ClinicFlow.Api.Infrastructure.Logging;

namespace ClinicFlow.Api.Features.Patients;

public static class PatientEndpoints
{
    /// <summary>
    /// The route group every patient endpoint starts from: shared prefix, Swagger tag and filters.
    /// In the JWT video we add .RequireAuthorization() here, once, for the whole group.
    /// </summary>
    public static RouteGroupBuilder MapPatientsGroup(this IEndpointRouteBuilder app) =>
        app.MapGroup("/api/patients")
           .WithTags("Patients")
           .AddEndpointFilter<RequestTimingFilter>();
}
