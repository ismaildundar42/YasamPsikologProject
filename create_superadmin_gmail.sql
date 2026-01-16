USE YasamPsikologDb;
GO

-- Create SuperAdmin user
-- Email: superadmin@gmail.com
-- Password: Admin123
-- Role: 0 (SuperAdmin)
INSERT INTO Users (FirstName, LastName, Email, PhoneNumber, PasswordHash, Role, Gender, IsActive, IsDeleted, CreatedAt, UpdatedAt)
VALUES (
    'Super',
    'Admin',
    'superadmin@gmail.com',
    '0000000000',
    'Admin123',
    0,
    0,
    1,
    0,
    GETDATE(),
    GETDATE()
);
GO

-- Verify
SELECT Id, FirstName, LastName, Email, Role, IsActive FROM Users WHERE Email = 'superadmin@gmail.com';
GO
