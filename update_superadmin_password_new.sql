USE YasamPsikologDb;
GO

-- Update SuperAdmin password
-- Email: superadmin@gmail.com
-- Password: Admin123
UPDATE Users
SET PasswordHash = '$2a$11$fJ8a3cVDFo2jjlXLjd34jO/.2ra6xi0MVuLPrSOkbKnVWI/t8Fpry'
WHERE Email = 'superadmin@gmail.com';
GO

-- Verify
SELECT Id, FirstName, LastName, Email, PasswordHash, Role, IsActive 
FROM Users 
WHERE Email = 'superadmin@gmail.com';
GO
