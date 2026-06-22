# Training Center API

A secure ASP.NET Core Web API for managing a training center. The system provides functionality for managing students, instructors, courses, enrollments, authentication, and authorization using JWT and policy-based access control.

## Overview

Training Center API is a layered ASP.NET Core application built around common training management workflows.

The project demonstrates:

* Clean separation between API, Application, Domain, and Infrastructure layers.
* JWT authentication with refresh token support.
* Role-based and resource-based authorization.
* Input validation using FluentValidation.
* Global exception handling.
* Swagger/OpenAPI documentation.
* Security-focused API design.

## Features

### Authentication & Security

* JWT Access Tokens
* Refresh Token Rotation
* Secure Refresh Token Hashing (BCrypt)
* Role-Based Authorization
* Resource-Based Authorization Policies
* Ownership Validation (Owner or Admin)
* Rate Limiting for Authentication Endpoints
* Global Exception Handling
* Structured Security Logging

### Student Management

* Create, update, delete, and retrieve students
* Student status management

  * Active
  * Suspended
  * Graduated
* Student profile management
* View student enrollments

### Instructor Management

* Create, update, delete, and retrieve instructors
* Instructor activation and deactivation
* Manager assignment
* Instructor hierarchy support
* View instructor courses

### Course Management

* Create, update, delete, and retrieve courses
* Course publishing workflow
* Draft, Published, and Archived states
* Course level filtering

  * Beginner
  * Intermediate
  * Advanced
* Instructor assignment

### Enrollment Management

* Enroll students into courses
* Track enrollment progress
* Complete enrollments with final grades
* Drop enrollments
* Enrollment statistics
* Student enrollment history
* Course enrollment history

---

## Architecture

The solution follows a layered architecture:

```text
TrainingCenter
│
├── TrainingCenter.API
│   ├── Controllers
│   ├── Authorization
│   ├── Middleware
│   └── Configuration
│
├── TrainingCenter.Application
│   ├── Services
│   ├── DTOs
│   ├── Validators
│   ├── Exceptions
│   └── Settings
│
├── TrainingCenter.Domain
│   ├── Entities
│   └── Enums
│
└── TrainingCenter.Infrastructure
    └── Entity Framework Core
```

### Responsibilities

| Layer          | Responsibility                                            |
| -------------- | --------------------------------------------------------- |
| API            | HTTP endpoints, authentication, authorization, middleware |
| Application    | Business rules, services, validation                      |
| Domain         | Entities and business models                              |
| Infrastructure | Database access and persistence                           |

---

## Domain Model

### Users

Authentication is centralized through the `User` entity.

Roles:

* Admin
* Instructor
* Student

### Core Entities

* User
* Student
* StudentProfile
* Instructor
* Course
* Enrollment

### Relationships

```text
User
 ├── Student
 └── Instructor

Student
 ├── StudentProfile
 └── Enrollments

Instructor
 └── Courses

Course
 └── Enrollments
```

---

## Authorization Model

The API uses both role-based and resource-based authorization.

### Roles

| Role       | Purpose                    |
| ---------- | -------------------------- |
| Admin      | Full system access         |
| Instructor | Instructor-owned resources |
| Student    | Student-owned resources    |

### Policies

* StudentOwnerOrAdmin
* InstructorOwnerOrAdmin
* CourseOwnerOrAdmin
* EnrollmentOwnerOrAdmin
* EnrollmentCourseInstructorOrAdmin

These policies ensure that users can access only resources they own unless they are administrators.

---

## Technologies

### Frameworks

* ASP.NET Core 9
* Entity Framework Core
* SQL Server

### Security

* JWT Bearer Authentication
* BCrypt.Net
* ASP.NET Authorization Policies
* Rate Limiting Middleware

### Validation

* FluentValidation

### Documentation

* Swagger / OpenAPI

---

## Getting Started

### Prerequisites

* .NET SDK 9.0
* SQL Server
* Visual Studio 2022 or later

### Clone the Repository

```bash
git clone <repository-url>
cd TrainingCenter
```

### Configure Database

Update the connection string in:

```json
appsettings.json
```

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=TrainingCenterDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Configure JWT

```json
{
  "JwtSettings": {
    "Key": "YOUR_SECRET_KEY",
    "Issuer": "TrainingCenterApi",
    "Audience": "TrainingCenterApiUsers",
    "ExpiryMinutes": 30,
    "RefreshTokenExpiryDays": 7
  }
}
```

### Apply Database Migrations

```bash
dotnet ef database update
```

### Run the Application

```bash
dotnet run
```

Swagger UI:

```text
https://localhost:<port>/swagger
```

---

## Authentication Flow

### Login

```http
POST /api/auth/login
```

Returns:

```json
{
  "accessToken": "...",
  "refreshToken": "...",
  "email": "user@example.com",
  "role": "Student",
  "expiresAt": "..."
}
```

### Refresh Token

```http
POST /api/auth/refresh
```

Generates a new access token and rotates the refresh token.

### Logout

```http
POST /api/auth/logout
```

Revokes the active refresh token.

---

## Validation

All incoming requests are validated using FluentValidation.

Examples:

* Email format validation
* String length constraints
* Enum validation
* Date validation
* Business rule validation

Invalid requests automatically return structured validation responses.

---

## Error Handling

The API uses centralized exception handling middleware.

Common responses:

| Status | Description           |
| ------ | --------------------- |
| 400    | Bad Request           |
| 401    | Unauthorized          |
| 403    | Forbidden             |
| 404    | Not Found             |
| 409    | Conflict              |
| 429    | Too Many Requests     |
| 500    | Internal Server Error |

---

## API Documentation

Swagger/OpenAPI is enabled in development mode.

Features:

* JWT authentication support
* Response documentation
* Endpoint summaries
* Schema generation

---

## Security Features

* JWT Authentication
* Refresh Token Rotation
* BCrypt Password Hashing
* Resource Ownership Validation
* Role-Based Authorization
* Policy-Based Authorization
* Rate Limiting
* Secure Logging
* Refresh Token Revocation
* Global Exception Handling

---

## Learning Objectives

This project demonstrates practical implementation of:

* REST API design
* ASP.NET Core architecture
* Entity Framework Core
* Authentication and Authorization
* Secure API development
* Validation and exception handling
* Resource-based access control
* API documentation practices

---

## License

This project is intended for educational and training purposes.
