using ClinicFlow.Api.Infrastructure.Endpoints;
using MediatR;

namespace ClinicFlow.Api.Features.Nationalities.ListNationalities;

public sealed class ListNationalitiesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // Lookup for client dropdowns: the codes here are what the patient endpoints accept.
        app.MapGet("/api/nationalities", async (ISender sender, CancellationToken ct) =>
            TypedResults.Ok(await sender.Send(new ListNationalitiesQuery(), ct)))
        .WithTags("Lookups")
        .WithName("ListNationalities")
        .WithSummary("List nationalities (ISO 3166-1), sorted by name");
    }
}
