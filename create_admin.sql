USE YasamPsikologDb;
GO

INSERT INTO Users (Email, PasswordHash, FirstName, LastName, PhoneNumber, Role, Gender, IsActive, IsDeleted, CreatedAt)
VALUES ('admin@yasampsikolog.com', 'Admin123', 'Admin', 'User', '5551234567', 0, 0, 1, 0, GETDATE());

SELECT Id, Email, PasswordHash, FirstName, LastName, Role FROM Users WHERE Email = 'admin@yasampsikolog.com';
GO
