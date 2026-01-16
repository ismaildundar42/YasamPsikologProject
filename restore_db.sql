USE master;
GO

RESTORE DATABASE YasamPsikologDb 
FROM DISK = '/var/opt/mssql/data/YasamPsikologDb.bak' 
WITH REPLACE;
GO
