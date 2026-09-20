# Final Presentation Outline — Task & Project Management API
*(BinX Tech .NET Backend Internship — Week 10)*

Structure per the Day 1 lesson: problem → architecture/decisions → biggest
challenge → performance work → outcome. Slide count is a guide, not a rule —
tighten or expand once you know your time slot.

---

## 1. Title Slide (1 slide)
- Project name, your name, "BinX Tech .NET Backend Internship — Capstone"
- One-line tagline, e.g. *"A secure, ownership-aware task & project management API."*

## 2. The Problem & Project Type (1 slide)
- What kind of system is this and who is it for (a team/organization tracking
  projects and tasks per user)
- Why a REST API + role/ownership model, not a simpler CRUD-only design
- [FILL IN: the specific angle you chose and why — what made this project
  yours rather than a generic tutorial CRUD app]

## 3. Architecture & Key Technical Decisions (2–3 slides)
- High-level diagram: Client → ASP.NET Core API → EF Core → SQL Server, with
  Redis alongside for caching
- **Auth:** ASP.NET Core Identity + JWT — why JWT over cookie sessions for an
  API-first design
- **Authorization:** role-based (`Admin` vs `User`) *and* ownership-based —
  a regular user only ever sees their own projects; `AdminWithEmail` policy
  gates admin-only routes
- **Data access:** EF Core with pagination/filtering/sorting on the projects
  list endpoint (`page`, `pageSize`, `name`, `sort`) — why this over returning
  every row
- **Caching:** Redis, cache-aside pattern — what's cached and why (hot read
  paths, not everything)
- **Cross-cutting:** custom `RequestTimingMiddleware`, centralized
  `ProblemDetails` error handling, FluentValidation, rate limiting on login
- [FILL IN: one decision you'd defend if challenged — e.g. "why SQL Server
  over an alternative," ready with your actual reasoning]

## 4. Biggest Challenge (1–2 slides)
This is the most personal slide — the program deliberately wants a real
engineering story here, not a polished list of features.
- [FILL IN: the specific bug or design problem that took the longest to solve]
- [FILL IN: how you diagnosed it — what you tried, what didn't work]
- [FILL IN: the actual fix and why it worked]

*A few candidates from your own build log, if useful as a starting point:*
- The ownership-through-relationship bug (`AppTask` has no direct `OwnerId`;
  ownership has to be checked via `Project.OwnerId`)
- The pagination bug where `totalCount` was computed before filters were
  applied instead of after
- The EF Core + `WebApplicationFactory` test-setup conflict (had to remove
  every EF Core service descriptor, not just `DbContextOptions`, before
  re-registering the in-memory provider for tests)

## 5. Performance Work — Sprint 3 (1–2 slides)
- N+1 query diagnosis and the EF Core projection fix that resolved it
- Redis cache-aside caching added to hot summary/read endpoints
- Database indexing added via Fluent API migrations
- [FILL IN: your actual before/after numbers from the Week 8 Day 4
  benchmarking session — response time, query count, or `STATISTICS IO/TIME`
  output. A concrete number here is worth more than the whole rest of this
  slide; don't leave it generic.]

## 6. Testing & CI/CD (1 slide)
- Unit tests (xUnit/Moq) + integration tests (WebApplicationFactory)
- GitHub Actions pipeline: build → test → (gated) deploy
- Definition of Done audit passed for both capstone projects in Week 9

## 7. Live Demo (no slide — switch to Swagger/Postman)
- Register → Login → create a project → show pagination/filtering →
  show a `403`/`404` when a non-owner tries to access another user's project
  → show the Admin-only route

## 8. Outcome & What's Next (1 slide)
- Final state: tested, secured, cached, CI/CD-covered, deployed API
- [FILL IN: deployed URL once confirmed]
- One line on what you'd add with more time (e.g. refresh tokens, more
  granular permissions, full versioning)

## 9. Q&A
- Reason from evidence on anything you don't have a ready answer for —
  the rubric explicitly rewards honest, evidence-based answers over a
  confident guess.

---

### Notes
- Keep the deck screenshots-and-diagrams-first; this outline is the spine,
  not the slide content itself.
- Share this outline with your mentor today for feedback before you build
  full slide content tomorrow (Day 1's Hands-On Lab, step 5).
