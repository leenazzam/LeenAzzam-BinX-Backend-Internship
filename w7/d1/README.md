# Week 7 — Day 1: Sprint 2 Planning & Identity Integration

Started Sprint 2 by planning the authentication and authorization work for both capstone projects.

## Sprint 2 Goal

Integrate **ASP.NET Core Identity**, prepare **JWT authentication**, and define the roles and permissions needed for each project.

## Projects

### 1. Task & Project Management API

* Continue from Sprint 1.
* Integrate Identity with the existing `AppDbContext`.
* Use **Admin** and **User** roles.
* Link projects to their Identity owner.
* Prepare endpoint permissions for Sprint 2.

### 2. Cardiac Patient Monitoring API

* Continue from the existing Identity setup.
* Link the `Patient` entity to `IdentityUser` using `IdentityUserId`.
* Use three roles:

  * **Admin**
  * **Doctor**
  * **Patient**
* Prepare role permissions for each API module.

## Identity Integration

Both projects use:

```text
ASP.NET Core Identity
Entity Framework Core
SQL Server
JWT Authentication
```

For the Cardiac project, `AppDbContext` already inherits from:

```csharp
IdentityDbContext
```

The `Patient` model was updated with:

```csharp
public string IdentityUserId { get; set; } = null!;
```

This creates the link between the Patient and the Identity user.

## Migration

Created a new migration for the Cardiac project:

```bash
dotnet ef migrations add LinkPatientToIdentity
```

Migration status:

```text
20260816154824_InitialCreate
20260816161127_AddIdentity
20260830131000_LinkPatientToIdentity 
```

The migration adds `IdentityUserId` to `Patients` and creates a foreign key to `AspNetUsers`.

## Roles & Permissions

### Task Management API

| Role  | Access                 |
| ----- | ---------------------- |
| User  | Own projects/tasks     |
| Admin | Full management access |

### Cardiac Patient Monitoring API

| Role    | Access                        |
| ------- | ----------------------------- |
| Admin   | Full access                   |
| Doctor  | Manage patients' medical data |
| Patient | View permitted personal data  |

## Sprint 2 Backlog

* [x] Plan Sprint 2
* [x] Integrate/verify Identity
* [x] Link Patient to Identity
* [x] Create and review migration
* [x] Apply migration
* [ ] Implement/complete JWT registration and login
* [ ] Apply role-based authorization
* [ ] Add ownership checks
* [ ] Implement custom middleware
* [ ] Test authentication and authorization


---

## Sprint 2 Backlog

The Sprint 2 backlog was planned as follows:

| Task | Day |
|------|-----|
| Integrate ASP.NET Core Identity with the existing DbContext | Day 1 |
| Add and apply Identity-related migrations | Day 1 |
| Define Admin, Doctor, and Patient roles | Day 1 |
| Link Patient with IdentityUser | Day 2 |
| Implement Patient registration and JWT login | Day 2 |
| Add role-based authorization | Day 3 |
| Add patient ownership checks | Day 3 |
| Implement custom middleware | Day 4 |
| Mentor code review | Day 4 |
| Postman authentication/RBAC demo | Day 5 |
| Sprint Review and Retrospective | Day 5 |

---

## Day 1 Result

Sprint 2 planning was completed, Identity integration was reviewed, the Patient entity was linked to Identity, and the new migration was created and reviewed. The next step is to apply the migration and continue with JWT registration and login.
