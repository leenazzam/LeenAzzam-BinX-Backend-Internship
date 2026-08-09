### Day 1 – ASP.NET Core Identity & Registration

Implemented ASP.NET Core Identity with Entity Framework Core and SQL Server. The database was updated with the Identity schema, and Identity services were configured in the application. A user registration endpoint was implemented using `UserManager.CreateAsync`, with validation for invalid passwords. Registration was tested in Postman using both a valid password and a deliberately weak password, confirming that invalid input is rejected with meaningful error messages.
