namespace ClinicFlow.Api.Infrastructure.Endpoints;

/// <summary>
/// One endpoint per use case. Every class that implements this is found by assembly scanning
/// and mapped by MapFeatureEndpoints(), so adding a use case never touches a shared file.
/// </summary>
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
