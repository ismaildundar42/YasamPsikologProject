USE YasamPsikologDb;
GO

-- Create SuperAdmin user
-- Email: superadmin@yasampsikolog.com
-- Password: Admin123!
INSERT INTO Users (FirstName, LastName, Email, PhoneNumber, PasswordHash, Role, Gender, IsActive, IsDeleted, CreatedAt, UpdatedAt)
VALUES (
    'Super',
    'Admin',
    'superadmin@yasampsikolog.com',
    '0000000000',
    '$2a$11$5ZqL5H5L5H5L5H5L5H5L5eO5L5H5L5H5L5H5L5H5L5H5L5H5L5H5O',
    'SuperAdmin',
    'Male',
    1,
    0,
    GETDATE(),
    GETDATE()
);
GO

-- Verify
SELECT Id, FirstName, LastName, Email, Role, IsActive FROM Users WHERE Email = 'superadmin@yasampsikolog.com';
GO
