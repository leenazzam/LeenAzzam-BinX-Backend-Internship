# Week 7 — Day 2: JWT Login & Registration

This is Day 2 of Sprint 2 in Phase 3 of the Task & Project Management API capstone.

## Objective

Implement the authentication flow for the capstone API by integrating ASP.NET Core Identity with JWT authentication, creating registration and login endpoints, and linking authenticated users to their domain data.

## Tasks Completed

* Linked the domain user data with the ASP.NET Core Identity user.
* Implemented user registration using `UserManager`.
* Created the corresponding domain user record during registration.
* Assigned the default `User` role to newly registered users.
* Implemented login using email and password validation.
* Generated JWT access tokens after successful authentication.
* Added relevant user information and domain-specific claims to the JWT.
* Configured JWT Bearer authentication for protected API endpoints.
* Tested the complete registration and login flow using Postman.
* Verified the created Identity user and domain data in the database.
* Verified the issued JWT and its claims.

## Authentication Flow

```text
Register
   ↓
Create IdentityUser
   ↓
Create Domain User
   ↓
Assign User Role
   ↓
Login
   ↓
Validate Credentials
   ↓
Generate JWT
   ↓
Access Protected Endpoints
```

## Technologies

* ASP.NET Core Identity
* JWT Bearer Authentication
* Entity Framework Core
* SQL Server
* Postman
* C#
* ASP.NET Core Web API

## Expected Result

The API now supports a complete authentication flow where users can register, log in, receive a JWT containing relevant claims, and use the token to access protected API endpoints.

## Day 2 Deliverable

A working registration and login system integrated with the Task & Project Management API, with JWT authentication and domain-specific user claims tested through Postman.
