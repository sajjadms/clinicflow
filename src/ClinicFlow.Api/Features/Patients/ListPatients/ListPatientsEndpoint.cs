using System.ComponentModel.DataAnnotations;
using ClinicFlow.Api.Infrastructure.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Features.Patients.ListPatients;

// [AsParameters] binds each property from the query string.
// The DataAnnotations are checked by the built-in .NET 10 validation (AddValidation).
public sealed record ListPatientsRequest(
    [property: FromQuery(Name = "page"), Range(1, int.MaxValue)] int Page = 1,
    [property: FromQuery(Name = "pageSize"), Range(1, 100)] int PageSize = 20,
    [property: FromQuery(Name = "includeInactive")] bool IncludeInactive = false);

public sealed class ListPatientsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatientsGroup().MapGet("", async (
            [AsParameters] ListPatientsRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var patients = await sender.Send(new ListPatientsQuery(request.Page, request.PageSize, request.IncludeInactive), ct);

            return TypedResults.Ok(patients);
        })
        .WithName("ListPatients")
        .WithSummary("List patients, sorted by name")
        .ProducesValidationProblem();
    }
}
