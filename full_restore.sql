USE master;
GO

-- Kill all connections
ALTER DATABASE YasamPsikologDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

-- Restore from backup
RESTORE DATABASE YasamPsikologDb 
FROM DISK = '/var/opt/mssql/data/YasamPsikologDb_new.bak' 
WITH REPLACE,
MOVE 'YasamPsikologDb' TO '/var/opt/mssql/data/YasamPsikologDb.mdf',
MOVE 'YasamPsikologDb_log' TO '/var/opt/mssql/data/YasamPsikologDb_log.ldf';
GO

-- Set multi user
ALTER DATABASE YasamPsikologDb SET MULTI_USER;
GO

-- Verify data
SELECT COUNT(*) as AppointmentCount FROM YasamPsikologDb.dbo.Appointments;
SELECT COUNT(*) as PsychologistCount FROM YasamPsikologDb.dbo.Psychologists;
SELECT COUNT(*) as ClientCount FROM YasamPsikologDb.dbo.Clients;
GO
