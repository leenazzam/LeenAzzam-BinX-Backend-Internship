# Week 5 — Testing, Error Handling & Project Kickoff

Improved the API with automated unit and integration testing, dependency mocking,
centralized exception handling, standardized error responses, and structured logging.

**Sequence:** `Project Scope → xUnit → Moq → Integration Testing → Error Handling`

## 🟢 Day 1 — Project Selection & Unit Testing with xUnit

[View Day 1](https://github.com/leenazzam/LeenAzzam-BinX-Backend-Internship/tree/main/w5/d1)

* Selected the **Task & Project Management API** as the Phase 3 capstone, extending the existing Weeks 1–4 project
* Defined the project scope and remaining work toward the Week 9 professional baseline
* Created a separate `WebApplication1.Tests` xUnit project
* Added unit tests using the **Arrange-Act-Assert** pattern with `[Fact]` and `[Theory]`
* Tested the `IsOverdue` business logic with multiple scenarios
* All **6 tests passed** successfully

## 🔵 Day 2 — Mocking Dependencies with Moq

[View Day 2](https://github.com/leenazzam/LeenAzzam-BinX-Backend-Internship/tree/main/w5/d2)

* Added `ITaskRepository` and separated `TaskService` from the real database
* Used **Moq** to replace the real repository with controlled test dependencies
* Tested successful repository responses and dependency exceptions
* Used `Verify` to confirm repository methods were called exactly once
* All **9 tests passed** successfully

## 🟡 Day 3 — Integration Testing with WebApplicationFactory

[View Day 3](https://github.com/leenazzam/LeenAzzam-BinX-Backend-Internship/tree/main/w5/d3)

* Configured `WebApplicationFactory` to run the API in-memory through the real HTTP pipeline
* Added integration tests for successful, not-found, and unauthorized responses
* Used an **In-Memory database** to keep integration tests isolated from the development database
* Tested protected endpoints using JWT authentication
* All **12 tests passed** successfully

## 🟠 Day 4 — Centralized Exception Handling

[View Day 4](https://github.com/leenazzam/LeenAzzam-BinX-Backend-Internship/tree/main/w5/d4)

* Implemented centralized exception handling using `UseExceptionHandler` in `Program.cs`
* Returned standardized **ProblemDetails** responses with HTTP `500`
* Prevented exception messages and stack traces from being exposed to clients
* Added structured logging with `ILogger` including the request path
* Tested the global handler with a temporary endpoint that intentionally threw an exception
* Removed unnecessary `try-catch` blocks from individual controllers

## 🔴 Day 5 — Testing & Week 5 Wrap-up

[View Day 5](https://github.com/leenazzam/LeenAzzam-BinX-Backend-Internship/tree/main/w5/d5)

* Applied xUnit and Moq to important authentication scenarios
* Added integration tests using `WebApplicationFactory` and an In-Memory database
* Configured `User` and `Admin` roles in the test environment
* Tested successful and failed login scenarios
* Ran the complete test suite using `dotnet test`

## Result

Week 5 established a proper testing and error-handling foundation for the API.

The final test suite contains **17 tests — all passed successfully**.

`17 Passed • 0 Failed • 0 Skipped`

The project is now ready for **Phase 3 — Sprint 1**.
