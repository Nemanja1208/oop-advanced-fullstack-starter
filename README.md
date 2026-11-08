> # ⚠️ ÖVNINGSBRANCH – SÅRBAR MED FLIT
>
> Den här branchen (`exercise/vecka-3-holes`) innehåller **medvetet inbyggda säkerhetshål**.
> Den är ett övningsmål för vecka 3 ("Tänk som en angripare"). **Kör bara lokalt. Deploya aldrig.**
> Lägg aldrig riktig data här. Läs `docs/vecka-3-las-mig-forst.md` innan du börjar.

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
  Starter.Benchmarks/   BenchmarkDotNet: measures the work-list sorting
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

All `/api/todos` endpoints read the caller's team from the `X-Team` request header (week 3). A
missing header falls back to the `utveckling` team. Real login arrives in week 4.

| Method | URL | Result |
|---|---|---|
| GET | `/health` | `200 { status: "ok" }` |
| GET | `/api/todos` | `200` list of todos |
| GET | `/api/todos/search?q=` | `200` todos whose title matches |
| POST | `/api/todos` | `201` created todo, `400` if the title is empty or too long |
| GET | `/api/todos/{id}` | `200` one todo, `404` if it doesn't exist |
| GET | `/api/todos/overdue` | `200` overdue todos with days late |
| GET | `/api/todos/stats` | `200` on-time / late / overdue counts and the on-time rate |
| PUT | `/api/todos/{id}` | `200` updated todo, `404` if it doesn't exist |
| PUT | `/api/todos/{id}/done` | `200` updated todo, `404` if it doesn't exist |
| DELETE | `/api/todos/{id}` | `200` deleted todo, `404` if it doesn't exist |

> The database schema changed in week 2. If the API fails to start or returns 500 after you pull, delete
> `backend/Starter.Api/starter.db` and start again (it is recreated automatically).

## Week 2: from requirement to number

- `docs/arendet.md`: the customer's request and bug report
- `docs/kravspec.md`: user stories, acceptance criteria, use case, non-functional requirements and metrics
- `docs/matning.md`: how the numbers were measured, and the results
- Every test is tagged with the requirement it proves: `dotnet test --filter "Krav=AK-2.1"`
- `Starter.Benchmarks/`: BenchmarkDotNet, `dotnet run -c Release --project Starter.Benchmarks`
- `perf/seed.mjs` + `npx autocannon`: load test of the API
