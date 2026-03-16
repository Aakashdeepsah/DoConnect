-- =============================================
-- DoConnect Database Schema (FINAL VERSION)
-- FIX 1: Idempotent — safe to run multiple times
-- FIX 2: CASCADE DELETE on Answers -> Questions
-- FIX 3: Admin seed only inserted if not exists
-- =============================================

-- Create database only if it does not exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'DoConnectDB')
BEGIN
    CREATE DATABASE DoConnectDB;
END
GO

USE DoConnectDB;
GO

-- =============================================
-- TABLE: Users
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        UserId       INT IDENTITY(1,1) PRIMARY KEY,
        Username     NVARCHAR(100)  NOT NULL UNIQUE,
        Email        NVARCHAR(200)  NOT NULL UNIQUE,
        PasswordHash NVARCHAR(500)  NOT NULL,
        Role         NVARCHAR(20)   NOT NULL DEFAULT 'User',
        CreatedAt    DATETIME       NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Users table created.';
END
GO

-- =============================================
-- TABLE: Questions
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Questions')
BEGIN
    CREATE TABLE Questions (
        QuestionId   INT IDENTITY(1,1) PRIMARY KEY,
        UserId       INT            NOT NULL,
        Topic        NVARCHAR(100)  NOT NULL,
        Title        NVARCHAR(300)  NOT NULL,
        QuestionText NVARCHAR(MAX)  NOT NULL,
        Status       NVARCHAR(20)   NOT NULL DEFAULT 'Pending',
        ImagePath    NVARCHAR(500)  NULL,
        CreatedAt    DATETIME       NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_Questions_Users FOREIGN KEY (UserId)
            REFERENCES Users(UserId) ON DELETE CASCADE
    );
    PRINT 'Questions table created.';
END
GO

-- =============================================
-- TABLE: Answers
-- FIX: ON DELETE CASCADE so deleting a question
--      automatically removes its answers
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Answers')
BEGIN
    CREATE TABLE Answers (
        AnswerId     INT IDENTITY(1,1) PRIMARY KEY,
        QuestionId   INT            NOT NULL,
        UserId       INT            NOT NULL,
        AnswerText   NVARCHAR(MAX)  NOT NULL,
        Status       NVARCHAR(20)   NOT NULL DEFAULT 'Pending',
        ImagePath    NVARCHAR(500)  NULL,
        CreatedAt    DATETIME       NOT NULL DEFAULT GETDATE(),
        -- FIX: CASCADE DELETE — when a question is deleted, its answers auto-delete
        CONSTRAINT FK_Answers_Questions FOREIGN KEY (QuestionId)
            REFERENCES Questions(QuestionId) ON DELETE CASCADE,
        CONSTRAINT FK_Answers_Users FOREIGN KEY (UserId)
            REFERENCES Users(UserId) ON DELETE NO ACTION
    );
    PRINT 'Answers table created.';
END
GO

-- =============================================
-- SEED: Admin account
-- FIX: Only inserts if admin does not already exist
-- Password = 'Admin@123' (BCrypt hash)
-- NOTE: This is the ONLY way to create an admin.
--       The register API always creates User role.
-- =============================================
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'admin@doconnect.com')
BEGIN
    INSERT INTO Users (Username, Email, PasswordHash, Role)
    VALUES (
        'admin',
        'admin@doconnect.com',
        '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy',
        'Admin'
    );
    PRINT 'Admin seed user created. Password: Admin@123';
END
ELSE
BEGIN
    PRINT 'Admin user already exists, skipping seed.';
END
GO

-- =============================================
-- VERIFY
-- =============================================
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE';
GO
