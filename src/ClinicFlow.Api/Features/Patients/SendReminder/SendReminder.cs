using System.Text.Json;
using ClinicFlow.Api.Infrastructure.Http;
using ClinicFlow.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Features.Patients.SendReminder;

public sealed record SendReminderCommand(Guid PatientId) : IRequest<ReminderResponse?>;

/// <summary>What we send back: the gateway's status, and what the gateway received (echoed back).</summary>
public sealed record ReminderResponse(
    int GatewayStatus,
    string? ContentType,
    Dictionary<string, string> HeadersReceived,
    JsonElement? BodyReceived);

public sealed class SendReminderHandler(
    ClinicFlowDbContext db,
    SmsGatewayClient smsGateway,
    ILogger<SendReminderHandler> logger) : IRequestHandler<SendReminderCommand, ReminderResponse?>
{
    private static readonly string[] HeadersToShow =
        ["content-type", "user-agent", "x-api-key", "x-correlation-id", "x-message-type"];

    public async Task<ReminderResponse?> Handle(SendReminderCommand request, CancellationToken cancellationToken)
    {
        var patient = await db.Patients
            .Where(p => p.Id == request.PatientId)
            .Select(p => new { p.Id, p.FirstName, p.MobileNumber })
            .SingleOrDefaultAsync(cancellationToken);

        if (patient is null)
        {
            return null;
        }

        var message = new SmsMessage(
            To: patient.MobileNumber,
            Text: $"Hello {patient.FirstName}, this is a reminder of your appointment at ClinicFlow tomorrow.");

        var result = await smsGateway.SendAsync(message, cancellationToken);

        // The patient id is fine to log; the mobile number is personal data, so it stays out of the logs.
        logger.LogInformation("Reminder for patient {PatientId} sent, gateway returned {GatewayStatus}",
            patient.Id, result.StatusCode);

        var headers = (result.Echo?.Headers ?? [])
            .Where(h => HeadersToShow.Contains(h.Key))
            .ToDictionary(h => h.Key, h => h.Key == "x-api-key" ? Mask(h.Value) : h.Value);

        return new ReminderResponse(result.StatusCode, result.ContentType, headers, result.Echo?.Json);
    }

    // Never return a secret: show only the first 4 characters.
    private static string Mask(string value) =>
        value.Length <= 4 ? "****" : value[..4] + new string('*', value.Length - 4);
}
