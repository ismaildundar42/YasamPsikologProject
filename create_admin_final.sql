USE YasamPsikologDb;
GO

-- Create SuperAdmin user
-- Email: admin@yasampsikolog.com
-- Password: Admin123!
-- Role: 0 = SuperAdmin, Gender: 0 = Male
INSERT INTO Users (FirstName, LastName, Email, PhoneNumber, PasswordHash, Role, Gender, IsActive, IsDeleted, CreatedAt, UpdatedAt)
VALUES (
    'Admin',
    'Yönetici',
    'admin@yasampsikolog.com',
    '5555555555',
    '$2a$11$5ZqL5H5L5H5L5H5L5H5L5eO5L5H5L5H5L5H5L5H5L5H5L5H5L5H5O',
    0,
    0,
    1,
    0,
    GETDATE(),
    GETDATE()
);
GO

-- Verify
SELECT Id, FirstName, LastName, Email, Role, Gender, IsActive FROM Users WHERE Email = 'admin@yasampsikolog.com';
GO
