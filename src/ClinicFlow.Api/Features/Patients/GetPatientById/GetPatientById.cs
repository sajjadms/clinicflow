using ClinicFlow.Api.Infrastructure.Persistence;
using MediatR;

namespace ClinicFlow.Api.Features.Patients.GetPatientById;

public sealed record GetPatientByIdQuery(Guid Id) : IRequest<PatientResponse?>;

public sealed class GetPatientByIdHandler(ClinicFlowDbContext db)
    : IRequestHandler<GetPatientByIdQuery, PatientResponse?>
{
    // One query: Patients joined with Nationalities, only the response columns.
    public Task<PatientResponse?> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken) =>
        db.GetPatientResponseAsync(request.Id, cancellationToken);
}
