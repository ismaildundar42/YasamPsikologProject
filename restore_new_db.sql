USE master;
GO

RESTORE DATABASE YasamPsikologDb 
FROM DISK = '/var/opt/mssql/data/YasamPsikologDb_new.bak' 
WITH REPLACE;
GO
