USE YasamPsikologDb;
GO

-- Update SuperAdmin password to plain text
-- Email: superadmin@gmail.com
-- Password: Admin123 (plain text)
UPDATE Users
SET PasswordHash = 'Admin123'
WHERE Email = 'superadmin@gmail.com';
GO

-- Verify
SELECT Id, FirstName, LastName, Email, PasswordHash, Role, IsActive 
FROM Users 
WHERE Email = 'superadmin@gmail.com';
GO
