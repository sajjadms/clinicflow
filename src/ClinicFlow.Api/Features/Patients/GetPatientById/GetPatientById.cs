using ClinicFlow.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Features.Patients.GetPatientById;

public sealed record GetPatientByIdQuery(Guid Id) : IRequest<PatientResponse?>;

public sealed class GetPatientByIdHandler(ClinicFlowDbContext db)
    : IRequestHandler<GetPatientByIdQuery, PatientResponse?>
{
    public async Task<PatientResponse?> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var patient = await db.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        return patient?.ToResponse();
    }
}
