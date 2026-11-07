# Fullstack Starter: .NET 10 API + React

A small, honest fullstack app to practise on: a todo list with a .NET 10 Web API backend and a
React (Vite + TypeScript) frontend. It is used in the course **Objektorienterad programmering, avancerad**
at NBI/Handelsakademin.

Click **Use this template → Create a new repository** to get your own copy.

## What's inside

```
backend/
  Starter.slnx
  Starter.Api/          .NET 10 Web API (controllers → service → repository → EF Core + SQLite)
  Starter.Tests/        xUnit: unit tests (fake repository) + integration tests (real API in memory)
frontend/               React 19 + Vite + TypeScript, Vitest, Prettier, oxlint
```

The request flow, from click to database:

```
React (TodosPage) → fetch → TodosController → TodoService → ITodoRepository → EF Core → SQLite
```

## Run it locally

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download) and [Node.js 22+](https://nodejs.org/).

**Backend** (http://localhost:5069)

```bash
cd backend
dotnet run --project Starter.Api
```

The SQLite database file `starter.db` is created automatically on first start.

**Frontend** (http://localhost:5173)

```bash
cd frontend
npm install
npm run dev
```

If your API runs on another port, copy `frontend/.env.example` to `frontend/.env.local` and change `VITE_API_URL`.

## Quality checks

| What | Backend | Frontend |
|---|---|---|
| Build | `dotnet build Starter.slnx` | `npm run build` |
| Tests | `dotnet test Starter.slnx` | `npm test` |
| Formatting | `dotnet format Starter.slnx --verify-no-changes` | `npm run format:check` |
| Lint | (compiler warnings) | `npm run lint` |

Right now **nothing runs these checks automatically**. Making GitHub do that on every Pull Request is the
first exercise of the course.

## Endpoints

| Method | URL | Result |
|---|---|---|
| GET | `/health` | `200 { status: "ok" }` |
| GET | `/api/todos` | `200` list of todos |
| POST | `/api/todos` | `201` created todo, `400` if the title is empty or too long |
| PUT | `/api/todos/{id}/done` | `200` updated todo, `404` if it doesn't exist |
