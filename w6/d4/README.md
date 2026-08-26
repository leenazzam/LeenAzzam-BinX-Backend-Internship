## Day 4 — Write Operations & Business Logic

Enhanced the `POST /api/tasks` endpoint by adding real business logic beyond simple field mapping, wrapped in a database transaction.

### Features

* Validates that the `ProjectId` exists before creating the task
* Prevents creating a task with a duplicate title within the same project
* Defaults `Status` to `Pending` when not provided
* Wraps the task creation in a database transaction using `BeginTransactionAsync`, `CommitAsync`, and `RollbackAsync`

### POST /api/tasks — Success

```http
POST /api/tasks
{
  "title": "Design homepage wireframe",
  "status": "Pending",
  "dueDate": "2026-09-01T00:00:00",
  "projectId": 1
}
```

Returns `201 Created` with the new task.

![POST /api/Tasks](image-2.png)

### Invalid ProjectId

```http
POST /api/tasks
{
  "projectId": 9999
}
```

Returns `404 Not Found` since the project does not exist.

![ProjectId](image.png)

### Duplicate Title

Sending the same title and projectId as an existing task returns `400 Bad Request`.

![Title](image-1.png)

All requests were tested through Swagger and returned the expected status codes.

<img width="1109" height="749" alt="image" src="https://github.com/user-attachments/assets/8edf3cf9-c113-4498-9e81-b72d0a9777b1" />
