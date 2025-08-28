# LoanOrigination.Api (Backend, .NET 8 Web API)

## Prereqs
- .NET 8 SDK
- (Optional) SQLite CLI

## Setup
```bash
cd backend/LoanOrigination.Api
dotnet restore
dotnet build
dotnet run
```
The API will start at `http://localhost:5000` (see `Properties/launchSettings.json`). Swagger UI: `http://localhost:5000/swagger`.

### Auth
- Seeded admin: `admin@local` / `Admin123$`
- Register a user at `POST /api/auth/register`
- Login at `POST /api/auth/login` → returns JWT
