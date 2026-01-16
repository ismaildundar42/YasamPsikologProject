USE master;
GO

-- Drop existing database
ALTER DATABASE YasamPsikologDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

DROP DATABASE YasamPsikologDb;
GO

-- Restore from backup with proper file paths
RESTORE DATABASE YasamPsikologDb 
FROM DISK = '/var/opt/mssql/data/YasamPsikologDb_new.bak' 
WITH REPLACE,
MOVE 'YasamPsikologDb' TO '/var/opt/mssql/data/YasamPsikologDb.mdf',
MOVE 'YasamPsikologDb_log' TO '/var/opt/mssql/data/YasamPsikologDb_log.ldf';
GO
