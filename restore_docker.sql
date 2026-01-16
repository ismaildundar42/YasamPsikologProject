-- Mevcut bağlantıları kes
USE master;
GO

-- Eğer veritabanı varsa sil
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'YasamPsikologDb')
BEGIN
    ALTER DATABASE YasamPsikologDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE YasamPsikologDb;
END
GO

-- Backup dosyasını restore et
RESTORE DATABASE YasamPsikologDb
FROM DISK = '/var/opt/mssql/data/YasamPsikologDb.bak'
WITH 
    MOVE 'YasamPsikologDb' TO '/var/opt/mssql/data/YasamPsikologDb.mdf',
    MOVE 'YasamPsikologDb_log' TO '/var/opt/mssql/data/YasamPsikologDb_log.ldf',
    REPLACE;
GO

-- Veritabanını kullanıma aç
ALTER DATABASE YasamPsikologDb SET MULTI_USER;
GO

SELECT 'Database restored successfully!' AS Status;
GO
