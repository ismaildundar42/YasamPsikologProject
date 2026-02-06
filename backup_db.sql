USE master;
GO

-- YasamPsikologDb veritabanının backup'ını al
BACKUP DATABASE YasamPsikologDb
TO DISK = '/var/opt/mssql/data/YasamPsikologDb_backup.bak'
WITH FORMAT, INIT, NAME = 'YasamPsikologDb-Full Database Backup', SKIP, STATS = 10;
GO

SELECT 'Backup completed successfully!' AS Status;
GO
