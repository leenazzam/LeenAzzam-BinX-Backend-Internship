# Week 7 — Sprint 2: Authentication & Role-Based Access

## Overview

Week 7 was **Sprint 2** of Phase 3.

The main focus was securing the capstone APIs by implementing:

* ASP.NET Core Identity
* JWT Authentication
* Role-Based Authorization
* Resource Ownership Checks
* Custom Middleware
* Postman Testing
* Sprint Review & Retrospective

The work was applied to:

1. **Task & Project Management API**
2. **Cardiac Patient Monitoring API**

---

## Days

### Day 1 — Sprint Planning & Identity Integration

The sprint started by defining the Sprint 2 goal and backlog.

### Main Work

* Integrated/verified ASP.NET Core Identity.
* Linked domain entities with Identity users.
* Updated the `Patient` entity with `IdentityUserId`.
* Created and reviewed the Identity migration.
* Defined the required roles.

### Roles

**Task Management API:**

* User
* Admin

**Cardiac Patient Monitoring API:**

* Patient
* Doctor
* Admin

### Migration

Created:

```text
LinkPatientToIdentity
```

The migration connects the `Patient` entity with `AspNetUsers`.

➡️ [Go to Day 1](./d1/README.md)

---

### Day 2 — JWT Login & Registration

The focus was implementing the authentication flow.

### Main Work

* Created Identity users using `UserManager`.
* Created the related domain user.
* Assigned the default `User` role.
* Implemented registration.
* Implemented login.
* Generated JWT access tokens.
* Added domain-specific claims.
* Configured JWT Bearer Authentication.
* Tested registration and login using Postman.

### Authentication Flow

```text
Register
   ↓
Identity User
   ↓
Domain User
   ↓
Role
   ↓
Login
   ↓
Validate Credentials
   ↓
Generate JWT
   ↓
Access Protected Endpoints
```

➡️ [Go to Day 2](./d2/README.md)

---

### Day 3 — RBAC & Ownership Checks

The focus was controlling what each authenticated user can access.

### Role-Based Authorization

**Cardiac API:**

```text
Admin
Doctor
Patient
```

**Task Management API:**

```text
Admin
User
```

Different endpoints were protected according to the user's role.

Example:

```csharp
[Authorize(Roles = "Admin")]
```

and:

```csharp
[Authorize(Roles = "Admin,Doctor")]
```

### Ownership Checks

Users cannot access resources belonging to other users.

For example:

```text
JWT
 ↓
PatientId / UserId
 ↓
Check resource owner
 ↓
Own resource → Allow
Other user's resource → Reject
```

### Testing

The authorization rules were tested using Postman.

Examples:

* Patient accessing own data → ✅
* Patient accessing another patient's data → `404 Not Found`
* User accessing another user's project → `404 Not Found`
* User accessing Admin endpoint → `403 Forbidden`
* Doctor accessing allowed endpoint → ✅

➡️ [Go to Day 3](./d3/README.md)

---

### Day 4 — Custom Middleware & Mentor Review

A custom middleware was implemented to handle a cross-cutting concern.

### RequestTimingMiddleware

The middleware measures request execution time and logs:

* HTTP method
* Request path
* Execution time in milliseconds

Flow:

```text
Request
   ↓
RequestTimingMiddleware
   ↓
Start Timer
   ↓
API Endpoint
   ↓
Calculate Time
   ↓
Log Result
```

The middleware was tested successfully in both projects.

Authorization and ownership checks were also reviewed in preparation for mentor code review.

➡️ [Go to Day 4](./d4/README.md)

---

### Day 5 — Sprint Review & Retrospective

The final day focused on reviewing and demonstrating Sprint 2.

### Postman Demo

The authentication and authorization flows were demonstrated.

Successful cases:

* Login
* Access own resources
* Admin access
* Doctor access

Rejected cases:

* Accessing another user's resource → `404`
* Accessing Admin endpoint without Admin role → `403`

### Retrospective

#### What went well?

* Identity and JWT authentication were implemented.
* Roles and ownership checks were applied.
* Authorization was tested using Postman.
* Middleware was implemented.

#### Challenges

* Integrating Identity with the existing database.
* Applying ownership checks consistently.
* Testing multiple roles and authorization scenarios.

#### What did I learn?

I learned how to use:

* ASP.NET Core Identity
* JWT Authentication
* RBAC
* Ownership Checks
* Custom Middleware

#### Sprint 3 Action Item

> Write an ownership-check test for every new resource endpoint.

➡️ [Go to Day 5](./d5/README.md)

---

# Week 7 Deliverables

* ✅ ASP.NET Core Identity integration
* ✅ Identity migration
* ✅ Domain entity linked to Identity
* ✅ JWT authentication
* ✅ Registration and login
* ✅ Role-Based Authorization
* ✅ Ownership checks
* ✅ Custom middleware
* ✅ Postman authorization testing
* ✅ Sprint Review
* ✅ Sprint Retrospective

## Next Step

**Week 8 — Sprint 3: Advanced Queries, Redis Caching & Performance Tuning**
