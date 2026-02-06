USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'YasamPsikologDb')
BEGIN
    ALTER DATABASE YasamPsikologDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE YasamPsikologDb;
END
GO

RESTORE DATABASE YasamPsikologDb
FROM DISK = '/var/opt/mssql/data/22ocak.bak'
WITH 
    MOVE 'YasamPsikologDb' TO '/var/opt/mssql/data/YasamPsikologDb.mdf',
    MOVE 'YasamPsikologDb_log' TO '/var/opt/mssql/data/YasamPsikologDb_log.ldf',
    REPLACE;
GO

ALTER DATABASE YasamPsikologDb SET MULTI_USER;
GO

SELECT 'Database restored successfully from 22ocak.bak!' AS Status;
GO
