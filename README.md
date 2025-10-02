# AdminFlow API

ASP.NET Core 8 REST API for the AdminFlow student management system. Handles authentication, user management, course management, enrollment tracking, and dashboard analytics.

## Stack

- ASP.NET Core 8 / C#
- Entity Framework Core 8
- SQL Server
- JWT Bearer authentication
- Swagger / OpenAPI

## Setup

1. Update `appsettings.json` with your SQL Server connection string
2. Run the app — it auto-migrates and seeds on first launch:

```bash
dotnet run --project AdminFlow.API
```

API runs at `https://localhost:7001` by default. Swagger UI at `/swagger`.

## Demo Logins

| Role    | Email                  | Password    |
|---------|------------------------|-------------|
| Admin   | admin@adminflow.io     | admin123    |
| Teacher | sarah@adminflow.io     | teacher123  |
| Student | james@student.io       | student123  |

## Endpoints

```
POST  /api/auth/login          Login
GET   /api/auth/me             Current user (protected)

GET   /api/users               List users (filter: role, search)
POST  /api/users               Create user (Admin)
PATCH /api/users/:id           Update user (Admin)
DELETE /api/users/:id          Delete user (Admin)

GET   /api/courses             List courses (filter: status, search)
POST  /api/courses             Create course (Admin)
PATCH /api/courses/:id         Update course (Admin, Teacher)
DELETE /api/courses/:id        Delete course (Admin)

GET   /api/enrollments         List enrollments (filter: courseId, studentId)
POST  /api/enrollments         Enroll student (Admin, Teacher)
PATCH /api/enrollments/:id     Update grade/status (Admin, Teacher)
DELETE /api/enrollments/:id    Remove enrollment (Admin)

GET   /api/stats               Dashboard statistics
```
