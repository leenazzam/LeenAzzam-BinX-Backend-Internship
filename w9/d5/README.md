# Day 5 — Definition of Done Audit, Sprint Review & Retrospective

## Overview

Day 5 was the final close-out day for Sprint 4 and Phase 3.

The main goal was to run the complete **Definition of Done (DoD) audit** against the finished projects, verify the actual implementation, fix any remaining gaps, and prepare the projects for the Week 10 final presentation.

The audit was performed for both projects:

* **Task Management API**
* **Cardiac Patient Monitoring API**

---

# 1. Definition of Done Audit

The Definition of Done checklist was reviewed item by item against the actual projects.

The audit focused on verifying the live implementation, documentation, database, deployment, CI/CD pipeline, and build quality.

## 1.1 Audit Checklist

| #  | Definition of Done Item                            | Task Management API | Cardiac Patient Monitoring API |
| -- | -------------------------------------------------- | ------------------- | ------------------------------ |
| 1  | All sprint tasks are completed                     | ✅                   | ✅                              |
| 2  | API runs end-to-end without critical errors        | ✅                   | ✅                              |
| 3  | Swagger/OpenAPI documentation is available         | ✅                   | ✅                              |
| 4  | Postman collection is available                    | ✅                   | ✅                              |
| 5  | At least one Postman test exists for each endpoint | ✅                   | ✅                              |
| 6  | Database schema is documented with an ERD          | ✅                   | ✅                              |
| 7  | Database is managed using EF Core migrations       | ✅                   | ✅                              |
| 8  | README contains setup instructions                 | ✅                   | ✅                              |
| 9  | README contains the technology stack               | ✅                   | ✅                              |
| 10 | Environment variables/configuration are documented | ✅                   | ✅                              |
| 11 | API documentation link is provided                 | ✅                   | ✅                              |
| 12 | Project is deployed                                | ✅                   | ✅                              |
| 13 | Public API URL is available                        | ✅                   | ✅                              |
| 14 | GitHub Actions CI/CD pipeline passes               | ✅                   | ✅                              |
| 15 | Project builds successfully                        | ✅                   | ✅                              |
| 16 | Compiler/nullable warnings are resolved            | ✅                   | ✅                              |

---

# 2. Task Management API Audit

The Task Management API was checked against the complete Definition of Done checklist.

## Verified Items

### Sprint Completion

All planned Sprint 4 tasks were reviewed and marked as completed.

### API

The API was tested through its documented endpoints and verified to run without critical errors.

### Swagger/OpenAPI

Swagger was checked to confirm that the API endpoints are documented and available for testing.

### Postman

The Postman collection was reviewed and contains requests and tests for the API endpoints.

### Database

The database structure was reviewed through the project's ERD.

EF Core migrations were also verified to ensure that the database schema is managed through migrations.

### README

The project README includes the required project information, including:

* Project description
* Technology stack
* Setup instructions
* Configuration/environment information
* API documentation
* Database information

### CI/CD

The GitHub Actions workflow was checked to verify that the latest build and test process passes successfully.

### Build Quality

The project was built and checked for compiler and nullable reference warnings.

---

# 3. Cardiac Patient Monitoring API Audit

The Cardiac Patient Monitoring API was also reviewed against the complete Definition of Done checklist.

## Verified Items

### Sprint Completion

All planned Sprint 4 tasks were reviewed and completed.

### API

The API was tested using the available endpoints and verified to run correctly without critical errors.

### Swagger/OpenAPI

Swagger was checked to confirm that the API endpoints are documented and available for testing.

### Postman

The Postman collection was reviewed and contains API requests and tests.

### Database

The database structure was reviewed using the ERD.

EF Core migrations were also verified.

### README

The project README contains the required documentation, including:

* Project overview
* Technology stack
* Setup instructions
* Configuration information
* API documentation
* Database information

### CI/CD

The GitHub Actions workflow was checked and the latest pipeline was verified to pass.

### Build Quality

The project was built and checked for compiler and nullable reference warnings.

---

# 4. Audit Result

After reviewing the Definition of Done checklist, both projects were considered ready for the final project phase.

The audit confirmed the main completion requirements:

* API functionality
* API documentation
* Database documentation
* EF Core migrations
* Postman testing
* README documentation
* Deployment
* CI/CD
* Build and code quality

Any issues identified during the audit were reviewed and addressed before closing Sprint 4.

---

# 5. Re-Testing After Fixes

After addressing the remaining gaps, the affected items were tested again.

The re-testing process included:

1. Running the API again.
2. Testing affected endpoints.
3. Checking Swagger documentation.
4. Running Postman tests.
5. Checking the database and migrations.
6. Checking the latest GitHub Actions workflow.
7. Building the projects again.
8. Confirming that the final state satisfies the Definition of Done.

The final checks were used to confirm that fixes did not introduce new problems.

---

# 6. Sprint 4 Review

The Sprint 4 Review focused on demonstrating that the projects were completed and ready for the next phase.

## Review Demonstration

The review included:

### 1. Definition of Done

The completed DoD checklist was presented and reviewed.

### 2. Live API

The deployed API was demonstrated through the available public endpoint.

### 3. Swagger

Swagger/OpenAPI was opened to demonstrate the available API endpoints.

### 4. Postman

Postman was used to demonstrate API requests and endpoint tests.

### 5. Database

The ERD and EF Core migration structure were reviewed.

### 6. GitHub Actions

The latest CI/CD workflow was shown to demonstrate that the project builds and tests successfully.

### 7. Documentation

The README documentation was reviewed to confirm that setup and API information is available.

---

# 7. Sprint 4 Retrospective

## What Went Well

* The main Sprint 4 tasks were completed.
* API documentation was improved.
* Postman testing was organized.
* Database documentation and migrations were reviewed.
* CI/CD pipelines were tested.
* README documentation was completed and improved.
* The projects were prepared for the final presentation phase.

## What Was Challenging

* Verifying every Definition of Done item against the actual implementation required checking several parts of the projects.
* Making sure the API, documentation, tests, database, deployment, and CI/CD were all consistent required multiple verification steps.
* Fixing remaining issues while keeping the projects stable required additional testing.

## What We Learned

* A project should not be considered complete only because the code is finished.
* The Definition of Done should be checked against the actual working system.
* Documentation, testing, deployment, and CI/CD are important parts of project completion.
* Re-testing after fixes is necessary to make sure the final changes work correctly.

## Action for Week 10

The main action for Week 10 is to prepare and practice the final project presentation.

The focus will be on:

* Explaining the project clearly.
* Demonstrating the main API features.
* Showing the architecture and database.
* Demonstrating testing and CI/CD.
* Explaining important technical decisions.
* Preparing a clear live demonstration.

---

# 8. Sprint 4 Summary

Sprint 4 focused on final verification and project close-out.

The projects were reviewed using the complete Definition of Done checklist instead of relying on previous assumptions about completion.

The final audit covered:

* Functionality
* API documentation
* Postman testing
* Database and migrations
* ERD
* README
* Deployment
* CI/CD
* Build quality
* Nullable/compiler warnings

After completing the audit and addressing the remaining gaps, the projects were prepared for the final presentation phase.

---

# 9. Phase 3 Completion

With the completion of Sprint 4, Phase 3 was closed.

The work completed during this phase prepared the projects for Week 10, where the main focus will move from development to final presentation and demonstration.

## Final Status

| Area                 | Status     |
| -------------------- | ---------- |
| Sprint Tasks         | ✅ Complete |
| API Functionality    | ✅ Verified |
| Swagger/OpenAPI      | ✅ Verified |
| Postman              | ✅ Verified |
| ERD                  | ✅ Verified |
| EF Core Migrations   | ✅ Verified |
| README               | ✅ Complete |
| Deployment           | ✅ Verified |
| CI/CD                | ✅ Verified |
| Build                | ✅ Verified |
| Code Quality         | ✅ Verified |
| Sprint Review        | ✅ Complete |
| Sprint Retrospective | ✅ Complete |
| Phase 3              | ✅ Closed   |
| Week 10 Preparation  | 🔄 Ready   |

---

# 10. Tools Used

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* Swagger / OpenAPI
* Postman
* GitHub
* GitHub Actions
* Docker
* Redis
* Notion
* Visual Studio / VS Code

---

# 11. Conclusion

Day 5 completed the Sprint 4 close-out process.

The Definition of Done was reviewed against the actual project implementation, remaining issues were addressed and re-tested, and the Sprint 4 Review and Retrospective were completed.

Both projects are now prepared for the final presentation phase in Week 10.
s