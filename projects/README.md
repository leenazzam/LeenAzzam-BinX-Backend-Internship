**# Cardiac Patient Monitoring System**

A REST API for managing cardiac patients, vital signs, medications, and appointments.

The project is built using **\*\*ASP.NET Core, Entity Framework Core, and SQL Server LocalDB\*\***.

**## Technologies**

\* ASP.NET Core Web API

\* Entity Framework Core

\* SQL Server LocalDB

\* ASP.NET Core Identity

\* JWT Authentication

\* FluentValidation

\* xUnit

\* Moq

\* WebApplicationFactory

\* Swagger / OpenAPI

\* CORS

\* Rate Limiting

**## Main Features**

\* Patient, Vital Sign, Medication, and Appointment management

\* JWT authentication and role-based authorization

\* Three roles: **\*\*Admin, Doctor, Patient\*\***

\* DTOs for API requests and responses

\* FluentValidation for input validation

\* Login rate limiting

\* Database migrations and seed data

\* Swagger for API testing

\* Automated unit, mocking, and integration tests

\* Centralized error handling with ProblemDetails

**## Getting Started**

Restore the packages:

\`\`\`bash

dotnet restore

\`\`\`

Apply the database migrations:

\`\`\`bash

dotnet ef database update

\`\`\`

Run the project:

\`\`\`bash

dotnet run

\`\`\`

Swagger is available automatically in Development mode.

**## Authentication & Roles**

The API uses **\*\*ASP.NET Core Identity + JWT\*\***.

A default Admin account is seeded:

\`\`\`text

Email: admin\@cardiac.com

Password: Admin123!

\`\`\`

\* **\*\*Admin\*\***: Full access, including managing patients and deleting records.

\* **\*\*Doctor\*\***: Can manage vital signs, medications, and appointments.

\* **\*\*Patient\*\***: Can view their permitted healthcare data.

New users registered through the normal registration endpoint are assigned the Patient role automatically. Doctor accounts can only be created by an Admin.

**## Roles & Permissions by Module**

\| Module       | GET all / GET by id    | POST          | PUT           | DELETE |

\| ------------ | ---------------------- | ------------- | ------------- | ------ |

\| Patients     | Admin, Doctor          | Admin         | Admin         | Admin  |

\| VitalSigns   | Admin, Doctor, Patient | Admin, Doctor | Admin, Doctor | Admin  |

\| Medications  | Admin, Doctor, Patient | Admin, Doctor | Admin, Doctor | Admin  |

\| Appointments | Admin, Doctor, Patient | Admin, Doctor | Admin, Doctor | Admin  |

**## Validation & Security**

Create and Update requests are validated using **\*\*FluentValidation\*\***.

Invalid data, such as an invalid age, phone number, heart rate, or past appointment date, returns a structured \`400 Bad Request\`.

The login endpoint is also protected with rate limiting. After 5 attempts per minute, additional requests return \`429 Too Many Requests\`.

**## API Testing**

The API was tested through Swagger using Admin, Doctor, and Patient accounts.

The testing covered:

\* Authentication and JWT tokens

\* Role-based authorization

\* CRUD operations

\* Validation errors

\* \`401\` and \`403\` authorization responses

\* \`404\` for missing resources

\* Login rate limiting with \`429\`

\* Different permissions for Admin, Doctor, and Patient

The database is also seeded with sample patients, vital signs, medications, and appointments for testing.

**### Testing Walkthrough**

Registering a new account confirms it defaults to the Patient role.

![register]\(image.png)

Logging in as the seeded Admin account returns a JWT token.

![login]\(image-1.png)

That token is used to authorize all further requests through the Swagger Authorize button.

![token]\(image-2.png)

Testing role restrictions on Patients, an unauthenticated or wrong-role request to GET patients correctly returns \`403\`.

![admin 403]\(image-3.png)

Once authorized as Admin, the same request succeeds with a \`200\` and returns the seeded patients.

![after login admin 200]\(image-4.png)

Creating a new patient as Admin succeeds with a \`201\`.

![post patient]\(image-5.png)

Updating that same patient returns a \`204\`.

![update]\(image-8.png)

Deleting the patient also returns a \`204\`.

![delete patient]\(image-6.png)

Fetching the deleted patient afterward correctly returns a \`404\`, confirming the delete actually took effect.

![id 3 deleted and 4 updated]\(image-7.png)

Sending intentionally invalid data, such as an age outside the allowed range or an invalid gender value, triggers FluentValidation and returns a structured \`400\` with a clear message for each failing field.

![FluentValidation]\(image-9.png)

Sending repeated login requests quickly triggers the rate limiter, returning a \`429\` after the fifth attempt within a minute.

![Rate Limiting]\(image-10.png)

The same CRUD pattern was verified across the other modules. Creating a vital sign record succeeds as expected.

![POST /api/vitalsigns]\(image-11.png)

Fetching all vital sign records confirms the new record is present.

![GET /api/vitalsigns]\(image-12.png)

Sending a heart rate outside the valid range correctly triggers a validation error.

![heartRate out of range validation]\(image-13.png)

Creating a medication record succeeds the same way.

![POST /api/medications]\(image-14.png)

Creating an appointment with a future date also succeeds.

![POST /api/appointments]\(image-15.png)

To verify the Doctor role specifically, an Admin account was used to create a Doctor account through the create-doctor endpoint.

![POST /api/auth/create-doctor]\(image-16.png)

That Doctor account was then used to log in and obtain its own token.

![doctor login]\(image-17.png)

A second account was registered normally to represent a Patient.

![patient register]\(image-18.png)

Logging in as that Patient and attempting a restricted action correctly returns \`403\`.

![patient 403]\(image-19.png)

Finally, logging in as the Doctor and attempting to create a patient, an action reserved for Admin only, also correctly returns \`403\`, confirming the role boundaries between Admin and Doctor are enforced as intended.

![doctor 403 in post patient]\(image-20.png)

**## Automated Testing**

The project includes automated tests using **\*\*xUnit, Moq, and WebApplicationFactory\*\***.

**### Unit Tests — xUnit**

Unit tests cover the VitalSign validation rules and verify both valid and invalid input scenarios.

The tests include cases such as valid heart rate values and invalid values outside the allowed range.



![xUnit Unit Tests]\(image-21.png)

**### Mocking — Moq**

Moq is used to isolate the VitalSign service logic from external dependencies and test the service behavior independently.



![Moq Service Tests]\(image-22.png)

**### Integration Tests — WebApplicationFactory**

Integration tests use \`WebApplicationFactory\` with an in-memory database to test the API through HTTP requests.

The tests cover authentication and authorization scenarios, including:

\* Unauthenticated requests returning \`401 Unauthorized\`

\* Successful Admin authentication

\* Authorized Admin access returning \`200 OK\`

\* Patient users being denied restricted operations with \`403 Forbidden\`

![Integration Tests]\(image-23.png)

**### Test Results**

The complete automated test suite was executed using:

\`\`\`bash

dotnet test

\`\`\`

All tests passed successfully.

![Test Results - 15 Passed]\(image-24.png)

**## Error Handling**

Unhandled exceptions are handled centrally using ASP.NET Core's \`UseExceptionHandler\`.

When an unexpected exception occurs:

\* The exception is caught by the centralized exception handler.

\* Full exception details are logged server-side using \`ILogger\`.

\* The client receives a standardized \`ProblemDetails\` response.

\* Internal exception messages and stack traces are not exposed to the client.

![Centralized Error Handling]\(image-25.png)

This provides consistent and safe error responses across the API while keeping internal implementation details protected.

**## Project Structure**

\`\`\`text

CardiacPatientMonitoring/

├── Controllers/

├── Models/

├── DTOs/

├── Validators/

├── Data/

├── Migrations/

├── Program.cs

├── appsettings.json

└── README.md

\`\`\`

**## Summary**

This project demonstrates building a secure and structured healthcare REST API using ASP.NET Core, with database management through EF Core, authentication using JWT, role-based authorization, validation, automated testing with xUnit and Moq, integration testing with WebApplicationFactory, centralized error handling with ProblemDetails, and API testing through Swagger.
