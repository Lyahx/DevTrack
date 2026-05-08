# DevTrack

A personal project & learning tracker. DevTrack helps you keep multiple parallel projects (APIs, web apps, games) and learning tracks (Claude tutorials, Coursera, books) organized so you can resume them after long breaks without losing context. This repository contains the full app: the .NET backend (Prompts 1 & 2 — infra, JWT auth, core CRUD, activity tracking, Resume Mode, Dashboard, Quick Capture, soft-delete cascade, daily reminder background service) plus the Next.js frontend (Prompt 3 — three-column shell, Resume Mode UI, Quick Capture, command palette, dark mode).

## Stack

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core 10 + Microsoft SQL Server 2022
- Scalar.AspNetCore (API docs)
- BCrypt.Net-Next + JWT Bearer
- Serilog (console + rolling file)
- FluentValidation, AutoMapper
- Docker / docker-compose

## Repo layout

```
DevTrack/
├── DevTrack.sln                 — backend solution
├── docker-compose.yml           — mssql + api
├── src/                         — .NET backend
│   ├── DevTrack.Api             — controllers, middleware, hosted services, Program.cs
│   ├── DevTrack.Service         — business logic, validators, activity tracker, resume/dashboard, reminder generator
│   ├── DevTrack.Repository      — EF-backed data access, OwnerScope, transaction factory
│   ├── DevTrack.Domain          — entities, enums, DTOs, exceptions, OwnerReference value object
│   └── DevTrack.Infrastructure  — DbContext, JWT, AutoMapper, migrations
└── web/                         — Next.js frontend (Prompt 3)
    ├── app/                     — App Router routes (login, register, dashboard, projects, learning, …)
    ├── components/              — shared UI: layout shell, owned-entity tabs, modals, common pieces
    ├── lib/api/                 — typed API client (one file per resource)
    ├── store/                   — Zustand stores (auth, command palette, quick capture)
    └── types/                   — TypeScript mirrors of the backend DTOs
```

## Run with Docker

Prereqs: Docker Desktop (with Compose v2).

```bash
cp .env.example .env
# Edit .env: set a strong MSSQL_SA_PASSWORD and a JWT_SECRET (>= 32 chars).
docker compose up --build
```

The API listens on `http://localhost:8080` (override with `API_HOST_PORT`). It auto-applies EF migrations on startup, so a fresh `DevTrack` database is created the first time the container boots.

API docs: open `http://localhost:8080/scalar/v1` in a browser.

## Run without Docker

Prereqs: .NET 10 SDK and a reachable MSSQL Server instance.

```powershell
# 1. Set environment variables (PowerShell)
$env:ConnectionStrings__Default = "Server=localhost,1433;Database=DevTrack;User Id=sa;Password=Your_password123;TrustServerCertificate=True;Encrypt=False"
$env:Jwt__Secret = "replace-with-a-long-random-secret-of-at-least-32-chars"

# 2. Apply migrations (first run only — startup also auto-migrates by default)
dotnet ef database update --project src/DevTrack.Infrastructure --startup-project src/DevTrack.Api

# 3. Run the API
dotnet run --project src/DevTrack.Api
```

To skip auto-migration on startup, set `DEVTRACK_AUTO_MIGRATE=false`.

## Migrations

Migrations live in `src/DevTrack.Infrastructure/Data/Migrations`. To add a new migration:

```bash
dotnet ef migrations add <Name> \
  --project src/DevTrack.Infrastructure \
  --startup-project src/DevTrack.Api \
  --output-dir Data/Migrations
```

A `DesignTimeDbContextFactory` lets `dotnet ef` work without booting the API — set `DEVTRACK_CONNECTION_STRING` if the default localhost fallback doesn't suit you.

## Auth flow

```http
POST /api/v1/auth/register   { username, email, password }
POST /api/v1/auth/login      { username, password }   → { token, expiresAt, user }
GET  /api/v1/auth/me         (Bearer token)
```

Send the token as `Authorization: Bearer <token>` for everything except `/auth/register`, `/auth/login`, and the Scalar docs page.

## Polymorphic ownership

Worklog / Decision / NextStep / Idea / Resource each belong to **exactly one** of: a Project, a Component, a Learning Track, or a Learning Module. The DB schema enforces this via a `CK_<Table>_Owner_ExactlyOne` check constraint on each table. Over the wire, requests and responses use a single `OwnerReference` object:

```json
{ "owner": { "type": "Component", "id": 5 }, "whatIDid": "..." }
```

`OwnerReference.FromColumns` / `.ToColumns` translate between the value object and the four nullable FK columns (`ProjectId`, `ComponentId`, `LearningTrackId`, `LearningModuleId`).

## LastActivityAt auto-update

When a Worklog / Decision / NextStep / Idea / Resource is **created**, `IActivityTrackingService.RecordActivityAsync` bumps `LastActivityAt` on the owner record. Component activity also bumps the parent Project; Module activity bumps the parent Track. Edits and deletes do **not** count as activity.

## Soft delete & cascade

`Project`, `Component`, `LearningTrack`, `LearningModule`, `User`, `Tag`, plus all five owned entities and `Reminder` carry `IsDeleted`/`DeletedAt`. EF Core global query filters hide deleted rows by default; pass `?includeDeleted=true` on listing endpoints to bypass.

Cascade rules (executed in a single DB transaction):
- **Project** delete → soft-deletes all its Components AND every owned record under the Project or any of its Components
- **Component** delete → soft-deletes its owned records (does not touch the parent Project)
- **LearningTrack** delete → soft-deletes its Modules AND every owned record under the Track or any of its Modules
- **LearningModule** delete → soft-deletes its owned records (does not touch the parent Track)

> TODO: implement soft-restore (un-delete) endpoints. Out of scope for Prompt 2.

## Endpoints

All paths are prefixed with `/api/v1`. Every response uses `{ "success", "data", "error" }`. All endpoints require `[Authorize]` except `/auth/register`, `/auth/login`, and `/scalar/v1`.

### Auth
- `POST /auth/register`
- `POST /auth/login`
- `GET  /auth/me`

### Projects
- `GET    /projects?status=&tagId=&page=&pageSize=&includeDeleted=`
- `GET    /projects/{id}`
- `POST   /projects`
- `PUT    /projects/{id}`
- `DELETE /projects/{id}` (soft delete; cascades)
- `POST   /projects/{id}/status`
- `POST   /projects/{id}/tags/{tagId}` / `DELETE /projects/{id}/tags/{tagId}`

### Components
- `GET    /projects/{projectId}/components`
- `GET    /components/{id}`
- `POST   /projects/{projectId}/components`
- `PUT    /components/{id}`
- `DELETE /components/{id}` (soft delete; cascades)
- `PUT    /components/{id}/status-note`
- `POST   /components/{id}/tags/{tagId}` / `DELETE /components/{id}/tags/{tagId}`

### Learning Tracks
- `GET    /learning-tracks?status=&tagId=&page=&pageSize=&includeDeleted=`
- `GET    /learning-tracks/{id}`
- `POST   /learning-tracks`
- `PUT    /learning-tracks/{id}`
- `DELETE /learning-tracks/{id}` (soft delete; cascades)
- `POST   /learning-tracks/{id}/status`
- `POST   /learning-tracks/{id}/tags/{tagId}` / `DELETE /learning-tracks/{id}/tags/{tagId}`

### Learning Modules
- `GET    /learning-tracks/{trackId}/modules`
- `GET    /modules/{id}`
- `POST   /learning-tracks/{trackId}/modules`
- `PUT    /modules/{id}`
- `DELETE /modules/{id}` (soft delete; cascades)
- `PUT    /modules/{id}/status`
- `PUT    /modules/{id}/order`

### Tags
- `GET    /tags`
- `POST   /tags`
- `PUT    /tags/{id}`
- `DELETE /tags/{id}` (soft delete)

### Worklogs
- `GET    /worklogs?projectId=&componentId=&learningTrackId=&learningModuleId=&fromDate=&toDate=&page=&pageSize=&includeDeleted=`
- `GET    /worklogs/{id}`
- `POST   /worklogs` (body carries `OwnerReference`; bumps owner `LastActivityAt`)
- `PUT    /worklogs/{id}`
- `DELETE /worklogs/{id}`
- `GET    /worklogs/recent?days=7`
- `GET    /projects/{id}/worklogs` · `/components/{id}/worklogs` · `/learning-tracks/{id}/worklogs` · `/modules/{id}/worklogs`

### Decisions
- `GET    /decisions?projectId=&componentId=&learningTrackId=&learningModuleId=&page=&pageSize=&includeDeleted=`
- `GET    /decisions/{id}`
- `POST   /decisions`
- `PUT    /decisions/{id}`
- `DELETE /decisions/{id}`
- `GET    /projects/{id}/decisions` · `/components/{id}/decisions` · `/learning-tracks/{id}/decisions` · `/modules/{id}/decisions`

### Next Steps
- `GET    /next-steps?...&isCompleted=&priority=`
- `GET    /next-steps/{id}`
- `POST   /next-steps`
- `PUT    /next-steps/{id}`
- `DELETE /next-steps/{id}`
- `PUT    /next-steps/{id}/complete` / `PUT /next-steps/{id}/uncomplete`
- `GET    /next-steps/open`
- `GET    /projects/{id}/next-steps` · `/components/{id}/next-steps` · `/learning-tracks/{id}/next-steps` · `/modules/{id}/next-steps`

### Ideas
- `GET    /ideas?...&isConvertedToNextStep=`
- `GET    /ideas/{id}`
- `POST   /ideas`
- `PUT    /ideas/{id}`
- `DELETE /ideas/{id}`
- `POST   /ideas/{id}/convert-to-next-step` (creates a NextStep with the same owner)
- `GET    /projects/{id}/ideas` · `/components/{id}/ideas` · `/learning-tracks/{id}/ideas` · `/modules/{id}/ideas`

### Resources
- `GET    /resources?...&type=ClaudeChat`
- `GET    /resources/{id}`
- `POST   /resources`
- `PUT    /resources/{id}`
- `DELETE /resources/{id}`
- `GET    /projects/{id}/resources` · `/components/{id}/resources` · `/learning-tracks/{id}/resources` · `/modules/{id}/resources`

### Reminders
- `GET    /reminders?isRead=&isDismissed=`
- `GET    /reminders/unread`
- `PUT    /reminders/{id}/read`
- `PUT    /reminders/{id}/dismiss`
- `DELETE /reminders/{id}`
- `POST   /reminders/run-generator` (manually invokes the generator)

### Resume Mode (single-call aggregations)
- `GET    /projects/{id}/resume`
- `GET    /components/{id}/resume`
- `GET    /learning-tracks/{id}/resume`
- `GET    /modules/{id}/resume`

### Dashboard
- `GET    /dashboard`

### Quick Capture
- `POST   /quick-capture` (thin alias over POST /ideas)

## Reminder background service

`ReminderGeneratorBackgroundService` is registered as a hosted service. It runs once when the API starts, then waits until the next **03:00 UTC** to run again, on a daily cadence. For each user it scans Active Projects and Active Learning Tracks; if `LastActivityAt` (or `CreatedAt` when LastActivityAt is null) is older than **14 days** AND no unread/undismissed reminder of the matching type already exists, it creates a Turkish-language reminder. Each iteration is wrapped in try/catch with Serilog logging — the service never crashes silently.

To trigger a run on demand, call `POST /api/v1/reminders/run-generator` (Bearer auth required).

## Frontend (`web/`)

Next.js 16 + TypeScript + Tailwind v4 + shadcn/ui (Base UI under the hood) + TanStack Query + Zustand + react-hook-form + zod + sonner + date-fns (Turkish locale) + next-themes.

### Run the frontend

```powershell
cd web
copy .env.local.example .env.local   # then edit if API URL differs
npm install
npm run dev                          # → http://localhost:3000
```

`.env.local`:
```
NEXT_PUBLIC_API_BASE_URL=http://localhost:8080/api/v1
```

The dev server expects the backend to be running (e.g. `docker compose up` from the repo root).

### Auth

JWT is stored in `localStorage` under `devtrack_token`. An axios interceptor attaches it automatically; on 401 the token is cleared and the user is redirected to `/login`. `<AuthGuard>` wraps the protected route group; `<GuestGuard>` blocks the auth pages once you're already logged in.

### Layout

Three-column shell on `xl` screens:
- **Header** (sticky): logo · command palette trigger · reminder bell with unread badge · user menu (theme toggle, settings, logout)
- **Sidebar** (left, 220px): dashboard, active projects, active learning tracks, reminders / tags / trash quick links — each section has a `+` for new
- **Main**: page content
- **Detail panel** (right, 320px, collapsible): "Open next steps" — global open list with inline complete (optimistic)
- **Floating Quick Capture FAB** (bottom-right): opens the capture modal

### Keyboard shortcuts

| Shortcut | Action |
| --- | --- |
| `Ctrl/⌘ + K` | Command palette (navigate to project / track / dashboard, theme toggle, logout) |
| `Ctrl/⌘ + J` | Quick Capture modal |
| `Ctrl/⌘ + Enter` | Submit Quick Capture |

### Pages

- `/login`, `/register` — public auth pages
- `/` — dashboard (greeting, stale projects banner, active projects/tracks, recent worklogs, high-priority next steps, unread reminders preview)
- `/projects`, `/projects/{id}` (Overview + Worklog/Decisions/NextSteps/Ideas/Resources tabs), `/projects/{id}/resume`
- `/components/{id}`, `/components/{id}/resume`
- `/learning`, `/learning/{id}` (Overview lists modules with inline status, plus tabs), `/learning/{id}/resume`
- `/learning/{id}/modules/{moduleId}`, `/learning/{id}/modules/{moduleId}/resume`
- `/reminders` — list with All / Unread / Dismissed filter, optimistic mark-read, "Şimdi tara" button hits `POST /reminders/run-generator`
- `/tags` — CRUD with color picker
- `/trash` — soft-deleted projects and tracks (restore is not implemented; the page says so)
- `/settings` — account info, theme toggle, shortcuts cheat sheet

### Optimistic updates

- Toggle a NextStep complete (dashboard, panel, tab) — UI updates immediately, rolls back on error
- Mark reminder read — same pattern
- Component status note save — debounced PUT
- Tag attach to project — optimistic via TanStack Query mutation

### Theme

Light mode by default. Persisted via `next-themes` (`localStorage`). Dark mode toggleable from the user menu, command palette, and `/settings`.

## Out of scope

Search endpoint, weekly digest, AI/LM Studio integration, soft-restore, automated tests, multi-language i18n (UI strings are inline Turkish).
