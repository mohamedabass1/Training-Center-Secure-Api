/*
===========================================================
Seed Data
===========================================================
*/

-----------------------------------------------------------
-- Users
-----------------------------------------------------------
INSERT INTO dbo.Users
(
    Email,
    PasswordHash,
    Role,
    IsActive
)
VALUES
('admin@trainingcenter.com', '$2a$11$x0qvI7KrFzdLwFlacQfWBOcmq9j3baiBZOcwLo5OjMdBmm9ABvZgK', 'Admin', 1),

('ahmed.instructor@trainingcenter.com', '$2a$12$InstructorHash1', 'Instructor', 1),
('sara.instructor@trainingcenter.com', '$2a$12$InstructorHash2', 'Instructor', 1),

('ali.student@trainingcenter.com', '$2a$12$StudentHash1', 'Student', 1),
('fatima.student@trainingcenter.com', '$2a$12$StudentHash2', 'Student', 1),
('mohammed.student@trainingcenter.com', '$2a$12$StudentHash3', 'Student', 1);
GO

-----------------------------------------------------------
-- Instructors
-----------------------------------------------------------
INSERT INTO dbo.Instructors
(
    UserId,
    FirstName,
    LastName,
    HireDate,
    Salary,
    ManagerId,
    IsActive
)
VALUES
(
    2,
    'Ahmed',
    'Al-Harbi',
    '2022-01-15',
    3500.00,
    NULL,
    1
),
(
    3,
    'Sara',
    'Al-Qahtani',
    '2023-03-10',
    3000.00,
    1,
    1
);
GO

-----------------------------------------------------------
-- Students
-----------------------------------------------------------
INSERT INTO dbo.Students
(
    UserId,
    FirstName,
    LastName,
    DateOfBirth,
    Status,
    PhoneNumber
)
VALUES
(
    4,
    'Ali',
    'Ahmed',
    '2001-05-10',
    'Active',
    '777111111'
),
(
    5,
    'Fatima',
    'Saleh',
    '2002-08-21',
    'Active',
    '777222222'
),
(
    6,
    'Mohammed',
    'Hassan',
    '2000-11-15',
    'Suspended',
    '777333333'
);
GO

-----------------------------------------------------------
-- Courses
-----------------------------------------------------------
INSERT INTO dbo.Courses
(
    Title,
    Code,
    Description,
    Price,
    Level,
    DurationHours,
    PublishedAt,
    Status,
    InstructorId
)
VALUES
(
    'C# Fundamentals',
    'CSHARP-101',
    'Introduction to C# Programming',
    99.99,
    'Beginner',
    30,
    GETDATE(),
    'Published',
    1
),
(
    'ASP.NET Core Web API',
    'ASPNET-201',
    'Build REST APIs using ASP.NET Core',
    149.99,
    'Intermediate',
    40,
    GETDATE(),
    'Published',
    1
),
(
    'SQL Server Essentials',
    'SQL-101',
    'Database Design and SQL Server',
    89.99,
    'Beginner',
    25,
    GETDATE(),
    'Published',
    2
);
GO

-----------------------------------------------------------
-- Student Profiles
-----------------------------------------------------------
INSERT INTO dbo.StudentProfiles
(
    StudentId,
    Address,
    City,
    Country,
    Bio,
    LinkedInUrl
)
VALUES
(
    1,
    'Al-Zubairi Street',
    'Sanaa',
    'Yemen',
    'Interested in Backend Development',
    'https://linkedin.com/in/ali'
),
(
    2,
    'Hadda',
    'Sanaa',
    'Yemen',
    'Learning ASP.NET Core',
    'https://linkedin.com/in/fatima'
),
(
    3,
    'Taiz Street',
    'Taiz',
    'Yemen',
    'Database Enthusiast',
    'https://linkedin.com/in/mohammed'
);
GO

-----------------------------------------------------------
-- Enrollments
-----------------------------------------------------------
INSERT INTO dbo.Enrollments
(
    StudentId,
    CourseId,
    EnrollmentDate,
    ProgressPercent,
    FinalGrade,
    Status
)
VALUES
(
    1,
    1,
    GETDATE(),
    100,
    95,
    'Completed'
),
(
    1,
    2,
    GETDATE(),
    40,
    NULL,
    'Active'
),
(
    2,
    1,
    GETDATE(),
    75,
    NULL,
    'Active'
),
(
    2,
    3,
    GETDATE(),
    100,
    88,
    'Completed'
),
(
    3,
    3,
    GETDATE(),
    10,
    NULL,
    'Dropped'
);
GO