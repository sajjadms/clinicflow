using ClinicFlow.Api.Infrastructure.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ClinicFlow.Api.Features.Patients.GenerateFileNo;

public sealed class GenerateFileNoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatientsGroup().MapPost("{id:guid}/file-number", async Task<Results<Ok<PatientResponse>, NotFound, ProblemHttpResult>> (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GenerateFileNoCommand(id), ct);

            return result.Status switch
            {
                GenerateFileNoStatus.NotFound => TypedResults.NotFound(),
                GenerateFileNoStatus.AlreadyAssigned => TypedResults.Problem(
                    title: "File number already assigned",
                    detail: $"This patient already has file number {result.Patient!.FileNo}.",
                    statusCode: StatusCodes.Status409Conflict),
                _ => TypedResults.Ok(result.Patient!)
            };
        })
        .WithName("GenerateFileNo")
        .WithSummary("Generate the patient's clinic file number")
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
