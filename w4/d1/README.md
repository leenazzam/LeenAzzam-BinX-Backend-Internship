### Day 1 – ASP.NET Core Identity & Registration

Implemented ASP.NET Core Identity with Entity Framework Core and SQL Server. The database was updated with the Identity schema, and Identity services were configured in the application. A user registration endpoint was implemented using `UserManager.CreateAsync`, with validation for invalid passwords. Registration was tested in Postman using both a valid password and a deliberately weak password, confirming that invalid input is rejected with meaningful error messages.

### Testing

The registration endpoint was tested using Postman with different input cases.

#### 1. Successful Registration

A valid email and strong password were provided. The user was successfully registered and the request returned a successful response.

<img width="1920" height="1008" alt="image" src="https://github.com/user-attachments/assets/756a60b6-05ae-4466-91c9-e30661961c0a" />


**Figure 1: Successful user registration using a valid password in Postman.**

#### 2. Weak Password Validation

A deliberately weak password was provided to verify ASP.NET Core Identity password validation. The request was rejected with a `400 Bad Request` response and meaningful validation errors.

<img width="1920" height="1008" alt="image" src="https://github.com/user-attachments/assets/4b9bdd24-94ef-4902-88e4-669c16d103b0" />


**Figure 2: Registration rejected due to an invalid password, with validation errors returned by ASP.NET Core Identity.**

### Result

The registration functionality was successfully implemented and tested. ASP.NET Core Identity correctly creates users with valid credentials and rejects invalid passwords according to the configured password requirements.
