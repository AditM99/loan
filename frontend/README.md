# Frontend (React + Vite + TS)

## Setup
```bash
cd frontend
npm i
# Point the API url to backend (defaults to http://localhost:5000)
echo "VITE_API_URL=http://localhost:5000" > .env.local
npm run dev
```

## Pages
- `/register` – create account
- `/login` – sign in (stores JWT)
- `/apply` – submit loan app, run AI prediction, upload doc
- `/` – your applications
- `/admin` – admin list & status updates (login as seeded admin)
