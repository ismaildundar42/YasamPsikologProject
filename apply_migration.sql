USE YasamPsikologDb;

-- Migration tablosunu kontrol et, yoksa oluştur
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END

-- MaxDailyPatients kolonunu ekle (idempotent)
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[dbo].[WorkingHours]') 
               AND name = 'MaxDailyPatients')
BEGIN
    ALTER TABLE WorkingHours ADD MaxDailyPatients int NULL;
    PRINT 'MaxDailyPatients kolonu eklendi';
END
ELSE
BEGIN
    PRINT 'MaxDailyPatients kolonu zaten mevcut';
END

-- Migration kaydını ekle
IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE MigrationId = '20260116000000_AddMaxDailyPatientsToWorkingHour')
BEGIN
    INSERT INTO [__EFMigrationsHistory] (MigrationId, ProductVersion)
    VALUES ('20260116000000_AddMaxDailyPatientsToWorkingHour', '8.0.17');
END

SELECT 'Migration tamamlandı' AS Status;
