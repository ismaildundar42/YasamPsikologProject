USE YasamPsikologDb;
GO

-- Create SuperAdmin user
-- Email: superadmin@yasampsikolog.com
-- Password: Admin123! (BCrypt hashed)
INSERT INTO Users (Email, PasswordHash, Role, IsActive, CreatedDate, UpdatedDate)
VALUES (
    'superadmin@yasampsikolog.com',
    '$2a$11$XGqZ8v0h.mKZqYqXQhZ5.O5QxXqGqQhZ5.O5QxXqGqQhZ5.O5QxXq',
    'SuperAdmin',
    1,
    GETDATE(),
    GETDATE()
);
GO

-- Verify
SELECT Id, Email, Role, IsActive FROM Users WHERE Email = 'superadmin@yasampsikolog.com';
GO
