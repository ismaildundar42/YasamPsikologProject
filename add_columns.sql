-- ConsultationFee ve ConsultationDuration kolonlarını ekle
USE [YasaMPsikologDb]
GO

-- ConsultationFee kolonu ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Psychologists') AND name = 'ConsultationFee')
BEGIN
    ALTER TABLE dbo.Psychologists
    ADD ConsultationFee DECIMAL(18,2) NOT NULL DEFAULT 0
END
GO

-- ConsultationDuration kolonu ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Psychologists') AND name = 'ConsultationDuration')
BEGIN
    ALTER TABLE dbo.Psychologists
    ADD ConsultationDuration INT NOT NULL DEFAULT 50
END
GO

-- Migration kaydını ekle
IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20251217143000_AddConsultationFeeAndDuration')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20251217143000_AddConsultationFeeAndDuration', '8.0.17')
END
GO

PRINT 'Migration başarıyla uygulandı!'
