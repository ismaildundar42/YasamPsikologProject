RESTORE DATABASE YasamPsikologDb 
FROM DISK = '/var/opt/mssql/data/yasam_db.bak' 
WITH REPLACE,
MOVE 'YasamPsikologDb' TO '/var/opt/mssql/data/YasamPsikologDb.mdf',
MOVE 'YasamPsikologDb_log' TO '/var/opt/mssql/data/YasamPsikologDb_log.ldf'
GO
