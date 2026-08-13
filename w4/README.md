# Week 4 — Authentication, Identity & Input Validation

Secured the REST API built in Week 3 with user identity, JWT authentication,
role-based authorization, input validation, and API hardening.

**Sequence:** `Identity → JWT → Authorization → Validation → Hardening`

## 🟢 Day 1 — ASP.NET Core Identity & User Registration
[View Day 1](./w4/d1)

- Added Identity NuGet packages; `AppDbContext` now inherits `IdentityDbContext`
- Added Identity tables via EF Core migration
- Registration endpoint using `UserManager.CreateAsync`
- Passwords are hashed automatically, never stored as plain text
- Tested valid registration and a weak-password rejection

## 🔵 Day 2 — JWT Authentication
[View Day 2](./w4/d2)

- Login endpoint verifies credentials via Identity
- Issues a signed JWT with user claims (ID, email) on success
- Configured JWT Bearer middleware: issuer, audience, signing key, expiry
- Tested valid/invalid logins and confirmed expired tokens are rejected

## 🟡 Day 3 — Authorization & Role-Based Access Control
[View Day 3](./w4/d3)

- Protected controllers with `[Authorize]`
- Created `User` and `Admin` roles; assigned via Identity
- Restricted `Delete` endpoint to `Admin` only
- Added a custom policy combining role + email claim
- Confirmed `401 Unauthorized` (no/invalid token) vs `403 Forbidden` (wrong role)

## 🟠 Day 4 — Input Validation with FluentValidation
[View Day 4](./w4/d4)

- Installed FluentValidation + ASP.NET Core integration
- Wrote validators for Create/Update requests with real business rules
  (e.g. required fields, allowed status values, future due dates)
- Validators run automatically before the controller executes
- Invalid requests return structured `400 Bad Request` errors

## 🔴 Day 5 — API Security & Hardening
[View Day 5](./w4/d5)

- **Rate limiting:** stricter limit on login vs general endpoints (brute-force protection)
- **CORS:** named policy allowing only the intended frontend origin
- **HTTPS/HSTS:** enforced secure transport
- **Security headers:** `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy`
- **SQL injection review:** confirmed EF Core/LINQ parameterizes all queries
- Tested rate limiting (`429`), CORS rejection, and expired JWT (`401`)

## Result

The API now verifies who the user is (Identity), how they prove it (JWT),
what they're allowed to do (Authorization), whether their input is valid
(FluentValidation), and how it's protected overall (Hardening).