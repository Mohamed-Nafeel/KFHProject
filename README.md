# KFH - Bank Account Management API

This solution implements a simplified Bank Account Management System with accounts and transfer requests.

## Setup
- Requires .NET 10 SDK.
- Restore packages: `dotnet restore`
- Run the API: `dotnet run --project KFH.csproj` or start from Visual Studio.
- By default the app uses a local SQLite DB file `kfh.db` in the working directory.

## Endpoints
- GET /api/accounts
- GET /api/accounts/{id}
- POST /api/accounts
- GET /api/transfers
- GET /api/transfers/{id}
- POST /api/transfers
- POST /api/transfers/{id}/process

## Design decisions
- Clean separation: Models, Data (DbContext), Controllers.
- SQLite selected for developer convenience; project includes EF Core and Sqlite provider.
- Minimal transaction handling in transfer processing to demonstrate correctness.
- OpenAPI enabled for development.

## NuGet Vulnerability Remediation
- Promoted Microsoft.OpenApi to explicit PackageReference (2.7.5) to address a transitive vulnerability.
- Added Microsoft.EntityFrameworkCore.Sqlite (10.0.9) to support local development with SQLite.

## Next steps
- Add repository/service abstractions and unit tests.
- Add FluentValidation and authentication for production use.

