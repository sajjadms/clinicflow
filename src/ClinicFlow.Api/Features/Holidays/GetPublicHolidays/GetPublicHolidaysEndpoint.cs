using System.Net;
using ClinicFlow.Api.Infrastructure.Endpoints;
using ClinicFlow.Api.Infrastructure.Http;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ClinicFlow.Api.Features.Holidays.GetPublicHolidays;

public sealed class GetPublicHolidaysEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // Clinic closed days: comes from an outside API (date.nager.at), not from our database.
        app.MapGet("/api/holidays/{year:int}/{countryCode}",
            async Task<Results<Ok<IReadOnlyList<PublicHoliday>>, NoContent, NotFound<string>>> (
                int year,
                string countryCode,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(new GetPublicHolidaysQuery(year, countryCode), ct);

                return result.Status switch
                {
                    HttpStatusCode.OK => TypedResults.Ok(result.Holidays),
                    HttpStatusCode.NoContent => TypedResults.NoContent(),
                    _ => TypedResults.NotFound($"Unknown country code '{countryCode}'.")
                };
            })
        .AllowAnonymous()
        .WithTags("Lookups")
        .WithName("GetPublicHolidays")
        .WithSummary("Public holidays for a year and country (from date.nager.at)");
    }
}
