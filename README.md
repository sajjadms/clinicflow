# ClinicFlow API

A production-style ASP.NET Core 10 Web API for outpatient clinics (patients, doctors, appointments), built step by step in a video series.

## Architecture

Single project, **Vertical Slice Architecture**: code is grouped by feature, not by technical layer.

```
src/ClinicFlow.Api/
├── Program.cs                 services in, pipeline out, endpoints mapped
├── Features/                  one folder per business capability
│   ├── FeatureExtensions.cs   AddFeatures() + MapFeatureEndpoints()
│   └── Patients/
│       ├── Patient.cs                 entity
│       ├── PatientResponse.cs         response DTO + mapping
│       ├── PatientEndpoints.cs        the /api/patients route group (prefix, tag, filters)
│       ├── CreatePatient/             command, handler, validator, endpoint
│       ├── GetPatientById/            query, handler
│       ├── ListPatients/              query, handler
│       ├── UpdatePatient/             request, command, handler, validator (PUT)
│       ├── DeactivatePatient/         soft delete (POST .../deactivate)
│       ├── DeletePatient/             hard delete (DELETE)
│       ├── GenerateFileNo/            clinic file number (POST .../file-number)
│       └── ChangePhoneNumber/         partial update (PATCH .../phone-number)
└── Infrastructure/            shared plumbing used by every feature
    ├── DependencyInjection.cs
    ├── Endpoints/             IEndpoint: one endpoint class per use case, auto-registered
    ├── Logging/               request timing endpoint filter (route group level)
    ├── Persistence/           EF Core DbContext
    ├── Validation/            FluentValidation endpoint filter
    └── ErrorHandling/         global exception handler (ProblemDetails)
```

When the app grows, the slices move into an Application project and the rest into Api, Domain and Infrastructure projects (Clean Architecture + VSA) without changing their shape.

## Stack

- .NET 10, minimal APIs
- MediatR 12 (request/handler per use case)
- FluentValidation 12 (endpoint filter)
- EF Core 10 (in-memory for now, with a named query filter for active patients; SQL Server later)
- Built-in OpenAPI document (`Microsoft.AspNetCore.OpenApi`, OpenAPI 3.1) with Swagger UI

## Run

```bash
dotnet run --project src/ClinicFlow.Api
```

Swagger UI: http://localhost:5080/swagger. OpenAPI document: http://localhost:5080/openapi/v1.json. Sample requests: `src/ClinicFlow.Api/ClinicFlow.Api.http`.

## Series

| Video | Topic |
| --- | --- |
| 1 | Vertical Slice Architecture: structure, minimal endpoints, MediatR, FluentValidation |
| 2 | MediatR in depth |
| 3 | Minimal APIs in depth: update, deactivate, delete, file numbers |
| 4 | JWT authentication, EF Core with SQL Server |
| 3 | Hangfire, Serilog, Polly, Azure Storage |
| 4 | OpenTelemetry and Grafana |
