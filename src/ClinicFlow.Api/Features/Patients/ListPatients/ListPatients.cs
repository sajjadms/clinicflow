using ClinicFlow.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Features.Patients.ListPatients;

public sealed record ListPatientsQuery(int Page, int PageSize) : IRequest<IReadOnlyList<PatientResponse>>;

public sealed class ListPatientsHandler(ClinicFlowDbContext db)
    : IRequestHandler<ListPatientsQuery, IReadOnlyList<PatientResponse>>
{
    private const int MaxPageSize = 100;

    public async Task<IReadOnlyList<PatientResponse>> Handle(ListPatientsQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);

        var patients = await db.Patients
            .AsNoTracking()
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return patients.Select(p => p.ToResponse()).ToList();
    }
}
