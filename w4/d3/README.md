## Day 3 Summary

Today, I worked on implementing JWT authentication in the ASP.NET Core Web API. I added the login endpoint, generated a signed JWT containing the user’s ID and email, configured JWT Bearer Authentication, and tested the login and protected endpoints using Postman.

### 1. Login — Valid Credentials ✅

A registered user was tested with valid email and password. The API successfully authenticated the user and returned a signed JWT token.

<img width="1920" height="1008" alt="Successful login with valid credentials" src="https://github.com/user-attachments/assets/90576988-4036-445a-9b90-d6ff10636929" />

**Figure 1: Successful login with valid credentials and JWT token returned by the API.**

### 2. Login — Wrong Password ❌

The login endpoint was tested using a valid email with an incorrect password. The API correctly rejected the request and returned `401 Unauthorized`.

<img width="1920" height="1008" alt="Login with wrong password" src="https://github.com/user-attachments/assets/1b3f27b6-970b-4e46-aecb-3b9fcd711469" />

**Figure 2: Login rejected due to an incorrect password.**

### 3. Login — User Not Found ❌

The login endpoint was tested using an email address that is not registered. The API correctly returned `401 Unauthorized`.

<img width="1920" height="1008" alt="Login with non-existing user" src="https://github.com/user-attachments/assets/be8df302-088a-43ad-93aa-0836c00d0b64" />

**Figure 3: Login rejected because the user does not exist.**

### 4. Protected Endpoint Without Token 🔒

A protected endpoint was accessed without providing a JWT token. The API correctly rejected the request and returned `401 Unauthorized`.

<img width="1920" height="1008" alt="Protected endpoint without token" src="https://github.com/user-attachments/assets/80fcecb4-5196-48be-b3f1-b09b4a3dead8" />

**Figure 4: Protected endpoint rejected when no JWT token is provided.**

### 5. Protected Endpoint With Valid Token ✅

The protected endpoint was accessed using a valid JWT token. The API successfully authenticated the request and returned the expected response.

<img width="1920" height="1008" alt="Protected endpoint with valid token" src="https://github.com/user-attachments/assets/9c643b8a-7640-4a97-b85c-0182bb5f257a" />

**Figure 5: Protected endpoint accessed successfully using a valid JWT token.**

### 6. JWT Token Environment Variable

The JWT token returned from the login response was automatically stored in a Postman environment variable using a Postman script. The stored token can then be reused in subsequent authenticated requests without manually copying and pasting it.

<img width="1920" height="1008" alt="JWT token environment variable" src="https://github.com/user-attachments/assets/4b6b52ed-94d3-4e2f-b5c0-d910e3a2bbb1" />

<img width="1920" height="1008" alt="JWT token script" src="https://github.com/user-attachments/assets/0b89d365-fbb6-4c10-8198-ceae2fde78e0" />

**Figure 6: JWT token automatically extracted from the login response and stored in the Postman environment variable using a script.**

### Result

JWT authentication was successfully implemented and tested. The API correctly authenticates valid users, rejects invalid credentials, protects authorized endpoints, and validates JWT tokens. Postman environment variables and scripts were also used to automatically store and reuse the generated JWT token.
