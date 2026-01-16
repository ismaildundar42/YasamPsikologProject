USE master;
GO

-- Drop existing database
ALTER DATABASE YasamPsikologDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

DROP DATABASE YasamPsikologDb;
GO

-- Restore from backup
RESTORE DATABASE YasamPsikologDb 
FROM DISK = '/var/opt/mssql/data/YasamPsikologDb_new.bak' 
WITH REPLACE;
GO
