# Day 5 — API Security & Hardening

Today, I secured the API using **Rate Limiting, CORS, HTTPS/HSTS, Security Headers, and SQL Injection protection**.

### Rate Limiting

Configured **5 requests/minute for Login** and **100 requests/minute for general endpoints**.

![alt text](<Screenshot 2026-08-13 134751.png>)

**Comment:** The API returned `429 Too Many Requests` after exceeding the allowed login requests.

### CORS

Configured a named `AllowFrontend` policy for `http://localhost:3000`.

![alt text](<Screenshot 2026-08-13 140437.png>)

**Comment:** The response contains `Access-Control-Allow-Origin: http://localhost:3000`, confirming that the allowed origin was accepted.

![alt text](<Screenshot 2026-08-13 140617.png>)

**Comment:** A request from an unauthorized origin did not receive an `Access-Control-Allow-Origin` header.

### Security Headers

Added security headers to API responses.

![alt text](<Screenshot 2026-08-13 141010.png>)
**Comment:** The response contains `X-Content-Type-Options`, `X-Frame-Options`, and `Referrer-Policy`, confirming that the security headers are enabled.

### JWT Expiration

Tested an expired JWT on a protected endpoint.

![alt text](<Screenshot 2026-08-13 140927.png>)
**Comment:** The API returned `401 Unauthorized` because the JWT had expired, confirming that `ValidateLifetime` is working correctly.

### SQL Injection Review

Reviewed the API queries and confirmed that EF Core/LINQ is used with parameterized queries, with no unparameterized raw SQL found.

**Result:** Day 5 security and API hardening requirements were completed and tested successfully.
