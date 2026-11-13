using ClinicFlow.Api.Infrastructure.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ClinicFlow.Api.Features.Patients.SendReminder;

public sealed class SendReminderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatientsGroup().MapPost("{id:guid}/reminders", async Task<Results<Ok<ReminderResponse>, NotFound>> (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var response = await sender.Send(new SendReminderCommand(id), ct);

            return response is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(response);
        })
        .WithName("SendReminder")
        .WithSummary("Send an appointment reminder SMS through the gateway");
    }
}
