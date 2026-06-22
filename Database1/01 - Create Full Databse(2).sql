/*
===========================================================
TrainingCenterDB
Full SQL Server Script
===========================================================
*/

-----------------------------------------------------------
-- 1) Create Database
-----------------------------------------------------------
IF DB_ID('TrainingCenterDB') IS NULL
BEGIN
    CREATE DATABASE TrainingCenterDB;
END
GO

USE TrainingCenterDB;
GO

-----------------------------------------------------------
-- 2) Drop Existing Tables
-----------------------------------------------------------
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL
    DROP TABLE dbo.Users;
GO

IF OBJECT_ID('dbo.Enrollments', 'U') IS NOT NULL
    DROP TABLE dbo.Enrollments;
GO

IF OBJECT_ID('dbo.StudentProfiles', 'U') IS NOT NULL
    DROP TABLE dbo.StudentProfiles;
GO

IF OBJECT_ID('dbo.Courses', 'U') IS NOT NULL
    DROP TABLE dbo.Courses;
GO

IF OBJECT_ID('dbo.Students', 'U') IS NOT NULL
    DROP TABLE dbo.Students;
GO

IF OBJECT_ID('dbo.Instructors', 'U') IS NOT NULL
    DROP TABLE dbo.Instructors;
GO

-----------------------------------------------------------
-- 3) Create Users
-----------------------------------------------------------
CREATE TABLE dbo.Users
(
    UserId INT IDENTITY(1,1) NOT NULL,

    Email NVARCHAR(150) NOT NULL,

    PasswordHash NVARCHAR(500) NOT NULL,

    Role NVARCHAR(20) NOT NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_Users_IsActive DEFAULT (1),

    RefreshTokenHash NVARCHAR(500) NULL,

    RefreshTokenExpiresAt DATETIME NULL,

    RefreshTokenRevokedAt DATETIME NULL,

    CONSTRAINT PK_Users
        PRIMARY KEY (UserId),

    CONSTRAINT UQ_Users_Email
        UNIQUE (Email),

    CONSTRAINT CK_Users_Role
        CHECK (Role IN ('Student', 'Instructor', 'Admin'))
);
GO

-----------------------------------------------------------
-- 4) Create Instructors
-----------------------------------------------------------
CREATE TABLE dbo.Instructors
(
    InstructorId INT IDENTITY(1,1) NOT NULL,

    UserId INT NOT NULL,

    FirstName NVARCHAR(50) NOT NULL,

    LastName NVARCHAR(50) NOT NULL,

    HireDate DATE NOT NULL,

    Salary DECIMAL(10,2) NOT NULL,

    ManagerId INT NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_Instructors_IsActive DEFAULT (1),

    CONSTRAINT PK_Instructors
        PRIMARY KEY (InstructorId),

    CONSTRAINT UQ_Instructors_UserId
        UNIQUE (UserId),

    CONSTRAINT CK_Instructors_Salary
        CHECK (Salary >= 0)
);
GO

-----------------------------------------------------------
-- 5) Create Students
-----------------------------------------------------------
CREATE TABLE dbo.Students
(
    StudentId INT IDENTITY(1,1) NOT NULL,

    UserId INT NOT NULL,

    FirstName NVARCHAR(50) NOT NULL,

    LastName NVARCHAR(50) NOT NULL,

    DateOfBirth DATE NOT NULL,

    RegisteredAt DATETIME NOT NULL
        CONSTRAINT DF_Students_RegisteredAt DEFAULT (GETDATE()),

    Status NVARCHAR(20) NOT NULL,

    PhoneNumber NVARCHAR(30) NULL,

    CONSTRAINT PK_Students
        PRIMARY KEY (StudentId),

    CONSTRAINT UQ_Students_UserId
        UNIQUE (UserId),

    CONSTRAINT CK_Students_Status
        CHECK (Status IN ('Active', 'Suspended', 'Graduated'))
);
GO

-----------------------------------------------------------
-- 6) Create Courses
-----------------------------------------------------------
CREATE TABLE dbo.Courses
(
    CourseId INT IDENTITY(1,1) NOT NULL,

    Title NVARCHAR(150) NOT NULL,

    Code NVARCHAR(30) NOT NULL,

    Description NVARCHAR(500) NULL,

    Price DECIMAL(10,2) NOT NULL,

    Level NVARCHAR(30) NOT NULL,

    DurationHours INT NOT NULL,

    CreatedAt DATETIME NOT NULL
        CONSTRAINT DF_Courses_CreatedAt DEFAULT (GETDATE()),

    PublishedAt DATETIME NULL,

    Status NVARCHAR(20) NOT NULL,

    InstructorId INT NOT NULL,

    CONSTRAINT PK_Courses
        PRIMARY KEY (CourseId),

    CONSTRAINT UQ_Courses_Code
        UNIQUE (Code),

    CONSTRAINT CK_Courses_Price
        CHECK (Price >= 0),

    CONSTRAINT CK_Courses_DurationHours
        CHECK (DurationHours > 0),

    CONSTRAINT CK_Courses_Level
        CHECK (Level IN ('Beginner', 'Intermediate', 'Advanced')),

    CONSTRAINT CK_Courses_Status
        CHECK (Status IN ('Draft', 'Published', 'Archived'))
);
GO

-----------------------------------------------------------
-- 7) Create StudentProfiles
-----------------------------------------------------------
CREATE TABLE dbo.StudentProfiles
(
    StudentId INT NOT NULL,

    Address NVARCHAR(200) NULL,

    City NVARCHAR(100) NULL,

    Country NVARCHAR(100) NULL,

    Bio NVARCHAR(500) NULL,

    LinkedInUrl NVARCHAR(200) NULL,

    CONSTRAINT PK_StudentProfiles
        PRIMARY KEY (StudentId)
);
GO

-----------------------------------------------------------
-- 8) Create Enrollments
-----------------------------------------------------------
CREATE TABLE dbo.Enrollments
(
    EnrollmentId INT IDENTITY(1,1) NOT NULL,

    StudentId INT NOT NULL,

    CourseId INT NOT NULL,

    EnrollmentDate DATETIME NOT NULL
        CONSTRAINT DF_Enrollments_EnrollmentDate DEFAULT (GETDATE()),

    CompletionDate DATETIME NULL,

    ProgressPercent DECIMAL(5,2) NOT NULL
        CONSTRAINT DF_Enrollments_ProgressPercent DEFAULT (0),

    FinalGrade DECIMAL(5,2) NULL,

    Status NVARCHAR(20) NOT NULL,

    CONSTRAINT PK_Enrollments
        PRIMARY KEY (EnrollmentId),

    CONSTRAINT UQ_Enrollments_StudentId_CourseId
        UNIQUE (StudentId, CourseId),

    CONSTRAINT CK_Enrollments_ProgressPercent
        CHECK (ProgressPercent >= 0 AND ProgressPercent <= 100),

    CONSTRAINT CK_Enrollments_FinalGrade
        CHECK (
            FinalGrade IS NULL
            OR (FinalGrade >= 0 AND FinalGrade <= 100)
        ),

    CONSTRAINT CK_Enrollments_Status
        CHECK (Status IN ('Active', 'Completed', 'Dropped'))
);
GO

-----------------------------------------------------------
-- 9) Foreign Keys
-----------------------------------------------------------

ALTER TABLE dbo.Instructors
ADD CONSTRAINT FK_Instructors_Manager
FOREIGN KEY (ManagerId)
REFERENCES dbo.Instructors(InstructorId)
ON DELETE NO ACTION
ON UPDATE NO ACTION;
GO

ALTER TABLE dbo.Instructors
ADD CONSTRAINT FK_Instructors_Users
FOREIGN KEY (UserId)
REFERENCES dbo.Users(UserId)
ON DELETE NO ACTION
ON UPDATE NO ACTION;
GO

ALTER TABLE dbo.Students
ADD CONSTRAINT FK_Students_Users
FOREIGN KEY (UserId)
REFERENCES dbo.Users(UserId)
ON DELETE NO ACTION
ON UPDATE NO ACTION;
GO

ALTER TABLE dbo.Courses
ADD CONSTRAINT FK_Courses_Instructors
FOREIGN KEY (InstructorId)
REFERENCES dbo.Instructors(InstructorId)
ON DELETE NO ACTION
ON UPDATE NO ACTION;
GO

ALTER TABLE dbo.StudentProfiles
ADD CONSTRAINT FK_StudentProfiles_Students
FOREIGN KEY (StudentId)
REFERENCES dbo.Students(StudentId)
ON DELETE CASCADE
ON UPDATE NO ACTION;
GO

ALTER TABLE dbo.Enrollments
ADD CONSTRAINT FK_Enrollments_Students
FOREIGN KEY (StudentId)
REFERENCES dbo.Students(StudentId)
ON DELETE CASCADE
ON UPDATE NO ACTION;
GO

ALTER TABLE dbo.Enrollments
ADD CONSTRAINT FK_Enrollments_Courses
FOREIGN KEY (CourseId)
REFERENCES dbo.Courses(CourseId)
ON DELETE CASCADE
ON UPDATE NO ACTION;
GO

-----------------------------------------------------------
-- 10) Indexes
-----------------------------------------------------------

CREATE INDEX IX_Instructors_ManagerId
ON dbo.Instructors (ManagerId);
GO

CREATE INDEX IX_Courses_InstructorId
ON dbo.Courses (InstructorId);
GO

CREATE INDEX IX_Students_Status
ON dbo.Students (Status);
GO

CREATE INDEX IX_Courses_Status
ON dbo.Courses (Status);
GO

CREATE INDEX IX_Enrollments_StudentId
ON dbo.Enrollments (StudentId);
GO

CREATE INDEX IX_Enrollments_CourseId
ON dbo.Enrollments (CourseId);
GO

CREATE INDEX IX_Enrollments_Status
ON dbo.Enrollments (Status);
GO