# AI-Powered Loan Origination – Full Stack

This repo contains a minimal but complete full-stack implementation:

- **Backend:** .NET 8 Web API, EF Core (SQLite), JWT auth, ML.NET AI eligibility
- **Frontend:** React + Vite + TypeScript

## Run

1) Backend
```bash
cd backend/LoanOrigination.Api
dotnet restore
dotnet run
```
The API will run at `http://localhost:5000`. Swagger: `http://localhost:5000/swagger`.

2) Frontend
```bash
cd frontend
npm i
echo "VITE_API_URL=http://localhost:5000" > .env.local
npm run dev
```

## Notes
- Seeded admin: `admin@local` / `Admin123$`
- File uploads are stored under `backend/LoanOrigination.Api/wwwroot/uploads`
- AI predictions are generated via ML.NET on-the-fly using a synthetic training dataset.
- The document extractor is a placeholder that pulls simple fields from the filename (e.g., `paystub_income_7500_name_John_Doe.pdf`). Replace with a real OCR provider later.
