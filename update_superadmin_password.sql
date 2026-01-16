USE YasamPsikologDb;
GO

-- Update SuperAdmin password
-- Email: superadmin@gmail.com
-- Password: Admin123
UPDATE Users 
SET PasswordHash = 'Admin123',
    UpdatedAt = GETDATE()
WHERE Email = 'superadmin@gmail.com';
GO

-- Verify
SELECT Id, FirstName, LastName, Email, PasswordHash, Role, IsActive FROM Users WHERE Email = 'superadmin@gmail.com';
GO
