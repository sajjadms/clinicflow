namespace ClinicFlow.Api.Features.Nationalities;

/// <summary>Lookup table: one row per ISO 3166-1 country. Patients reference it by NationalityId.</summary>
public sealed class Nationality
{
    public int Id { get; init; }

    public string Code { get; init; } = "";

    public string Name { get; init; } = "";
}
