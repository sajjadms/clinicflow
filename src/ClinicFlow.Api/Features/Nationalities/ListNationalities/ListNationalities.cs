using ClinicFlow.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Features.Nationalities.ListNationalities;

public sealed record NationalityResponse(string Code, string Name);

public sealed record ListNationalitiesQuery : IRequest<IReadOnlyList<NationalityResponse>>;

public sealed class ListNationalitiesHandler(ClinicFlowDbContext db)
    : IRequestHandler<ListNationalitiesQuery, IReadOnlyList<NationalityResponse>>
{
    public async Task<IReadOnlyList<NationalityResponse>> Handle(ListNationalitiesQuery request, CancellationToken cancellationToken) =>
        await db.Nationalities
            .AsNoTracking()
            .OrderBy(n => n.Name)
            .Select(n => new NationalityResponse(n.Code, n.Name))
            .ToListAsync(cancellationToken);
}
