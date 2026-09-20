# Task & Project Management API

A REST API for managing projects and tasks, with ownership-based access control so
each user only sees and manages their own projects (unless they're an Admin).

Built with **ASP.NET Core, Entity Framework Core, and SQL Server**.

## Technologies

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity + JWT Authentication
- Role & ownership-based authorization
- FluentValidation
- Redis (cache-aside caching)
- Custom `RequestTimingMiddleware`
- Centralized error handling with `ProblemDetails`
- Rate limiting (login endpoint)
- CORS
- xUnit, Moq, WebApplicationFactory
- Swagger / OpenAPI

## Main Features

- **Auth:** Register / Login with JWT issuance; new users are assigned the default
  `User` role
- **Projects:** Full CRUD, paginated + filterable + sortable (`page`, `pageSize`,
  `name`, `sort`)
- **Ownership-based access:** A regular user only sees/edits their own projects;
  Admins see everything (`AdminWithEmail` policy for admin-only endpoints)
- **Tasks:** Full CRUD, with task deletion restricted to the `Admin` role
- **Task Assignments:** links tasks to users
- **Redis caching** on hot read paths (cache-aside pattern)
- **Custom middleware** for request timing/logging
- **Centralized exception handling** — internal errors never leak stack traces to
  the client
- **Rate limiting** on the login endpoint to slow down brute-force attempts
- **Automated tests** — unit tests (xUnit/Moq) and integration tests
  (WebApplicationFactory)

## Getting Started

### 1. Restore the packages

```bash
dotnet restore
```

### 2. Apply the database migrations

```bash
dotnet ef database update
```

### 3. Run the project

```bash
dotnet run
```

Swagger is available automatically in Development mode.

### Configuration

Connection strings, the Redis endpoint, and the JWT signing key are read from
`appsettings.json` / `appsettings.Development.json`. **Before deploying or sharing
this repo publicly, move the JWT key and connection strings to user-secrets or
environment variables** — see the Notes section below.

## Authentication & Roles

The API uses **ASP.NET Core Identity + JWT**.

| Role | Access |
|---|---|
| `User` (default on registration) | Can create/view/edit/delete their own projects and tasks |
| `Admin` | Full access — can view all users' projects (`/api/projects/admin`), and is the only role that can delete tasks |

## API Endpoints (summary)

| Endpoint | Method | Auth | Notes |
|---|---|---|---|
| `/api/auth/register` | POST | — | Creates a user, assigns `User` role |
| `/api/auth/login` | POST | — | Rate-limited; returns a JWT |
| `/api/projects` | GET | ✅ | Paginated, filterable (`name`), sortable |
| `/api/projects/{id}` | GET | ✅ | Owner or Admin only |
| `/api/projects` | POST | ✅ | Creates a project owned by the caller |
| `/api/projects/{id}` | PUT | ✅ | Owner or Admin only |
| `/api/projects/{id}` | DELETE | ✅ | Owner or Admin only |
| `/api/projects/admin` | GET | ✅ Admin | All projects, any owner |
| `/api/projects/summary` | GET | ✅ | Aggregate summary |
| `/api/tasks` | GET/POST/PUT | ✅ | Task CRUD |
| `/api/tasks/{id}` | DELETE | ✅ Admin | Restricted to Admins |

Full request/response contracts are documented in Swagger.

## Project Structure

```text
WebApplication1/
│
├── Controllers/     → AuthController, ProjectsController, TasksController
├── DTOs/
├── Middleware/       → RequestTimingMiddleware
├── Migrations/
├── models/           → Project, AppTask, TaskAssignment
├── Repositories/
├── Services/
├── Validators/
├── data/
├── Program.cs
├── appsettings.json
└── README.md
```

## Summary

This project demonstrates a structured, secure ASP.NET Core backend with JWT
authentication, ownership-based authorization, paginated/filterable listing
endpoints, Redis caching, centralized error handling, and automated testing —
built and hardened progressively across the internship's four sprints.
