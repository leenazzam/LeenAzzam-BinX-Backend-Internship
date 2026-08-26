## Day 3 — Catalog & Read Operations

Enhanced the `GET /api/tasks` endpoint by adding pagination, filtering, sorting, and DTO projection.

### Features

* Pagination using `page` and `pageSize`
* Filtering by `status` and `projectId`
* Sorting by `title` or `dueDate`
* DTO projection using `TaskResponseDto`
* Paginated response using `PagedResult<T>`

### GET /api/tasks

Before updating the endpoint:

![GET /api/tasks](image.png)

After adding pagination and DTO response:

![GET AFTER UPDATE](image-1.png)

### Filtering

```http
GET /api/tasks?status=Pending
```

![GET /api/tasks?status=Pending](image-2.png)

### Pagination, Filtering & Sorting

```http
GET /api/tasks?page=1&pageSize=5&status=Pending&sort=dueDate
```

![Pagination, Filtering & Sorting](image-3.png)

### Pagination Example

```http
GET /api/tasks?page=2&pageSize=2
```

![page=2\&pageSize=2](image-4.png)

All requests were tested through Swagger and returned `200 OK` with the expected data.

<img width="1689" height="767" alt="image" src="https://github.com/user-attachments/assets/0f8299d6-187f-41c3-88d2-f4003285e2c1" />
