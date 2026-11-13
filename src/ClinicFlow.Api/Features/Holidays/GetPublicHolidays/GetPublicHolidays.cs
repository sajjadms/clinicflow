using ClinicFlow.Api.Infrastructure.Http;
using MediatR;

namespace ClinicFlow.Api.Features.Holidays.GetPublicHolidays;

public sealed record GetPublicHolidaysQuery(int Year, string CountryCode) : IRequest<HolidaysResult>;

public sealed class GetPublicHolidaysHandler(HolidaysClient holidays)
    : IRequestHandler<GetPublicHolidaysQuery, HolidaysResult>
{
    // The handler only asks the typed client: no URLs, no HttpClient setup here.
    public Task<HolidaysResult> Handle(GetPublicHolidaysQuery request, CancellationToken cancellationToken) =>
        holidays.GetPublicHolidaysAsync(request.Year, request.CountryCode.ToUpperInvariant(), cancellationToken);
}
