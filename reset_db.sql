-- Reset database, keep only superadmin
USE YasamPsikologDb;
GO

-- Delete in correct order due to foreign keys
DELETE FROM Appointments;
DELETE FROM WorkingHours;
DELETE FROM Clients;
DELETE FROM Psychologists;

-- Delete all users except superadmin
DELETE FROM Users 
WHERE Email != 'superadmin@gmail.com';

-- Verify remaining data
SELECT 'Remaining Users' as TableName, COUNT(*) as Count FROM Users
UNION ALL
SELECT 'Remaining Psychologists', COUNT(*) FROM Psychologists
UNION ALL
SELECT 'Remaining Clients', COUNT(*) FROM Clients
UNION ALL
SELECT 'Remaining Appointments', COUNT(*) FROM Appointments
UNION ALL
SELECT 'Remaining WorkingHours', COUNT(*) FROM WorkingHours;
GO
