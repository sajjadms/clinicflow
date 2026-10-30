# ClinicFlow API

A production-style ASP.NET Core 10 Web API for outpatient clinics (patients, doctors, appointments), built step by step in a video series.

## Architecture

Single project, **Vertical Slice Architecture**: code is grouped by feature, not by technical layer.

```
src/ClinicFlow.Api/
├── Program.cs                 services in, pipeline out, endpoints mapped
├── Features/                  one folder per business capability
│   ├── FeatureExtensions.cs   AddFeatures() + MapFeatureEndpoints()
│   ├── Nationalities/         ISO 3166-1 lookup table (seeded) + GET /api/nationalities
│   └── Patients/
│       ├── Patient.cs                 entity (+ Address and Guardian complex types)
│       ├── PatientEnums.cs            Gender, MaritalStatus, IdentityType, ...
│       ├── PatientDetails.cs          shared create/update input + mapping
│       ├── PatientDetailsValidator.cs one set of rules for create and update
│       ├── PatientResponse.cs         response DTO + EF Core projection
│       ├── PatientEndpoints.cs        the /api/patients route group (prefix, tag, filters)
│       ├── CreatePatient/             command, handler, validator, endpoint
│       ├── GetPatientById/            query, handler
│       ├── ListPatients/              query, handler
│       ├── UpdatePatient/             request, command, handler, validator (PUT)
│       ├── DeactivatePatient/         soft delete (POST .../deactivate)
│       ├── DeletePatient/             hard delete (DELETE)
│       ├── GenerateFileNo/            clinic file number (POST .../file-number)
│       └── ChangeMobileNumber/        partial update (PATCH .../mobile-number)
└── Infrastructure/            shared plumbing used by every feature
    ├── DependencyInjection.cs
    ├── Endpoints/             IEndpoint: one endpoint class per use case, auto-registered
    ├── Logging/               request timing endpoint filter (route group level)
    ├── Persistence/           EF Core DbContext, entity configurations, migrations
    ├── Validation/            FluentValidation endpoint filter
    └── ErrorHandling/         global exception handler (ProblemDetails)
```

When the app grows, the slices move into an Application project and the rest into Api, Domain and Infrastructure projects (Clean Architecture + VSA) without changing their shape.

## Stack

- .NET 10, minimal APIs
- MediatR 12 (request/handler per use case)
- FluentValidation 12 (endpoint filter)
- EF Core 10 with SQL Server (migrations, complex types, a named query filter for active patients)
- Built-in OpenAPI document (`Microsoft.AspNetCore.OpenApi`, OpenAPI 3.1) with Swagger UI

## Run

Set the connection string once (it is never committed), create the database, then run:

```bash
dotnet tool restore
dotnet user-secrets set "ConnectionStrings:ClinicFlow" "Server=localhost;Database=ClinicFlow;Trusted_Connection=True;TrustServerCertificate=True" --project src/ClinicFlow.Api
dotnet ef database update --project src/ClinicFlow.Api
dotnet run --project src/ClinicFlow.Api
```

No SQL Server on your machine? Run one in Docker (`mcr.microsoft.com/mssql/server`) and use a SQL login in the connection string.

Swagger UI: http://localhost:5080/swagger. OpenAPI document: http://localhost:5080/openapi/v1.json. Sample requests: `src/ClinicFlow.Api/ClinicFlow.Api.http`.

## Series

| Video | Topic |
| --- | --- |
| 1 | Vertical Slice Architecture: structure, minimal endpoints, MediatR, FluentValidation |
| 2 | MediatR in depth |
| 3 | Minimal APIs in depth: update, deactivate, delete, file numbers |
| 4 | Production-ready patient model, EF Core with SQL Server, migrations |
| 5 | JWT authentication |
| 6 | EF Core in depth: relationships, query optimization |
| 7 | Hangfire, Serilog, Polly, Azure Storage |
| 8 | OpenTelemetry and Grafana |
