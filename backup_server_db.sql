BACKUP DATABASE YasamPsikologDb
TO DISK = '/var/opt/mssql/data/YasamPsikologDb_backup_latest.bak'
WITH FORMAT, INIT, NAME = 'YasamPsikologDb-Full', SKIP, STATS = 10;
GO
