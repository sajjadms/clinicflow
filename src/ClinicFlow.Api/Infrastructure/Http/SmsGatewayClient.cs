using System.Text.Json;

namespace ClinicFlow.Api.Infrastructure.Http;

public sealed record SmsMessage(string To, string Text);

/// <summary>What the demo gateway (Postman Echo) sends back: the headers and the JSON body it received.</summary>
public sealed record EchoResponse(Dictionary<string, string> Headers, JsonElement? Json, string Url);

public sealed record SmsSendResult(int StatusCode, string? ContentType, DateTimeOffset? Date, EchoResponse? Echo);

/// <summary>Typed client for the SMS gateway (base URL https://postman-echo.com/).</summary>
public sealed class SmsGatewayClient(HttpClient http)
{
    public async Task<SmsSendResult> SendAsync(SmsMessage message, CancellationToken ct)
    {
        // 1. The request: method + URL
        using var request = new HttpRequestMessage(HttpMethod.Post, "post");

        // 2. A header for this request only (default headers were set at registration)
        request.Headers.Add("X-Message-Type", "appointment-reminder");

        // 3. The content (body): our object as JSON, with Content-Type: application/json
        request.Content = JsonContent.Create(message);
        // Short form for all of the above: await http.PostAsJsonAsync("post", message, ct);

        // 4. Send it and read the response: status code, headers, body
        using var response = await http.SendAsync(request, ct);

        var contentType = response.Content.Headers.ContentType?.MediaType;
        var date = response.Headers.Date;
        var echo = response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<EchoResponse>(ct)
            : null;

        return new SmsSendResult((int)response.StatusCode, contentType, date, echo);
    }

    /// <summary>Demo only: asks the gateway for a given status code, e.g. 503, to show retries.</summary>
    public async Task<int> CheckStatusAsync(int statusCode, CancellationToken ct)
    {
        using var response = await http.GetAsync($"status/{statusCode}", ct);
        return (int)response.StatusCode;
    }
}
