# Leen Azzam — BinX Tech .NET Backend Internship

A 10-week backend development internship at BinX Tech, building two production-style
ASP.NET Core Web APIs from scratch: a **Task & Project Management API** and a
**Cardiac Patient Monitoring API**. The repo tracks the full progression week by week —
from C# fundamentals through authentication, testing, performance optimization, and a
CI/CD pipeline to deployment.

## 🚀 Capstone Projects

| Project | Description | Docs |
|---|---|---|
| **Task & Project Management API** | Manages projects, tasks, and task assignments with role/ownership-based authorization | [`projects/TaskManagement/README.md`](projects/TaskManagement/README.md) |
| **Cardiac Patient Monitoring API** | Manages patients, vital signs, medications, and appointments with role-based access (Admin/Doctor/Patient) | [`projects/CardiacPatientMonitoring/README.md`](projects/CardiacPatientMonitoring/README.md) |

**Live demo:** _add the deployed API URL(s) here once confirmed_

## 🛠️ Tech Stack

- **Backend:** ASP.NET Core Web API (.NET 9), EF Core, SQL Server
- **Auth:** ASP.NET Core Identity, JWT bearer authentication, role & ownership-based authorization
- **Validation:** FluentValidation
- **Caching:** Redis (cache-aside pattern)
- **Testing:** xUnit, Moq, WebApplicationFactory (integration tests)
- **Docs:** Swagger / OpenAPI, Postman collections
- **CI/CD:** GitHub Actions (build + test on every PR)
- **Other:** Centralized error handling (`ProblemDetails`), custom request-timing middleware, rate limiting, CORS

## 📁 Repository Structure

```
projects/                  → stable, final versions of both capstone APIs
w1 .. w9/                  → weekly progression (w[n]/d[n] = week n, day n)
                              each day folder documents that day's work and code
```

The `projects/` folder is what a reviewer should look at for the finished product.
The `w1`–`w9` folders are the week-by-week learning record — proof of how the two
projects were actually built, sprint by sprint.

## 📈 Program Progression

| Phase | Weeks | Focus |
|---|---|---|
| Foundations | 1–2 | C# fundamentals, LINQ, async, ASP.NET Core basics |
| Core API Skills | 3–4 | REST + EF Core, Identity/JWT authentication, RBAC |
| Quality | 5 | Unit testing (xUnit/Moq), integration testing, centralized error handling |
| Sprint 1 — Build | 6 | Task & Project Management API: CRUD, EF Core Fluent API, seed data |
| Sprint 2 — Secure | 7 | JWT auth, RBAC, ownership-based authorization, custom middleware |
| Sprint 3 — Optimize | 8 | N+1 query fixes, Redis caching, indexing, benchmarking |
| Sprint 4 — Ship | 9 | CI/CD pipeline, Definition of Done audit, deployment |
| Final | 10 | Presentation, code review, certification |

## 👩‍💻 About Me

Computer Engineering student at Palestine Polytechnic University. This repository
represents 10 weeks of hands-on backend engineering — from an empty console app in
Week 1 to two tested, secured, cached, and deployed production APIs.

- GitHub: [@leenazzam](https://github.com/leenazzam)
