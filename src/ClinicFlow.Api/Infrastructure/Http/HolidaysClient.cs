using System.Net;

namespace ClinicFlow.Api.Infrastructure.Http;

public sealed record PublicHoliday(DateOnly Date, string LocalName, string Name, string CountryCode);

public sealed record HolidaysResult(HttpStatusCode Status, IReadOnlyList<PublicHoliday> Holidays);

/// <summary>
/// Typed client for the public holidays API (base URL https://date.nager.at/).
/// The HttpClient is created by IHttpClientFactory and injected here: we never "new" it.
/// </summary>
public sealed class HolidaysClient(HttpClient http)
{
    public async Task<HolidaysResult> GetPublicHolidaysAsync(int year, string countryCode, CancellationToken ct)
    {
        // Relative URL: the base address is set once, at registration.
        using var response = await http.GetAsync($"api/v3/PublicHolidays/{year}/{countryCode}", ct);

        // Check the status code ourselves, instead of letting an exception decide.
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var holidays = await response.Content.ReadFromJsonAsync<List<PublicHoliday>>(ct) ?? [];
            return new HolidaysResult(HttpStatusCode.OK, holidays);
        }

        if (response.StatusCode is HttpStatusCode.NoContent or HttpStatusCode.NotFound)
        {
            return new HolidaysResult(response.StatusCode, []);
        }

        response.EnsureSuccessStatusCode();   // anything else is a real failure
        return new HolidaysResult(response.StatusCode, []);
    }
}
