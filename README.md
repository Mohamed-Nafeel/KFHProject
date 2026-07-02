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

## Security / Secrets
- The API uses JWT Bearer authentication. Configure secrets via dotnet user-secrets or environment variables.
- Required settings (example keys):
  - Jwt:Key (symmetric signing key) -- store as secret
  - Jwt:Issuer
  - Jwt:Audience
  - Jwt:ExpiresMinutes

Example (local dev):
1. Initialize user secrets: `dotnet user-secrets init`
2. Set a secret key: `dotnet user-secrets set "Jwt:Key" "<strong-random-secret>"`
3. Optionally set issuer/audience: `dotnet user-secrets set "Jwt:Issuer" "KFH"`

You can also set environment variables in Production (e.g., Azure Key Vault, Azure App Configuration, or OS env vars).

