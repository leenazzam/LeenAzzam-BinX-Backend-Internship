# Day 4 — Input Validation with FluentValidation

In Day 4, FluentValidation was added to the existing ASP.NET Core API to validate Create and Update Task requests. The validators check both required fields and business rules such as valid task status, future due dates, and valid project IDs. Invalid requests are automatically rejected with a structured `400 Bad Request` response containing clear validation messages.

## Validation Tests

### 1. Valid Create Request

A valid Task was created with a valid title, status, future due date, and project ID. The API returned a successful response.
![alt text](<Screenshot 2026-08-12 213455.png>)

### 2. Missing Status

The request was sent with an empty Status value. The API returned `400 Bad Request` and indicated that Status is required.

![alt text](<Screenshot 2026-08-12 213736.png>)

### 3. Invalid Status

The request used an unsupported status value such as `Cancelled`. The API returned `400 Bad Request` with a message explaining that Status must be `Pending`, `In Progress`, or `Completed`.

![alt text](<Screenshot 2026-08-12 213751.png>)
### 4. Invalid DueDate

The request used a due date in the past. The API returned `400 Bad Request` because the DueDate must be in the future.
![alt text](<Screenshot 2026-08-12 213807.png>)
### 5. Invalid ProjectId

The request used `ProjectId = 0`. The API returned `400 Bad Request` because the ProjectId must be greater than 0.
![alt text](<Screenshot 2026-08-12 213828.png>)

### 6. Missing Title

The request was sent with an empty Title. The API returned `400 Bad Request` and indicated that Title is required.
![alt text](<Screenshot 2026-08-12 213855.png>)

### 7. Multiple Validation Errors

A request containing multiple invalid fields was tested. The API returned `400 Bad Request` and returned the validation errors for each invalid field in a structured format.
![alt text](<Screenshot 2026-08-12 213912.png>)

### 8. Valid Update Request

A valid Update Task request was tested using valid values for all required fields. The API processed the request successfully.
![alt text](<Screenshot 2026-08-12 213646.png>)
## Result

FluentValidation successfully validates both Create and Update requests and returns clear, structured validation errors before invalid requests are processed by the controller.
