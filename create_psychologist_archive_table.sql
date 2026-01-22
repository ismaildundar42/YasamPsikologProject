-- PSİKOLOG ARŞİV TABLOSU
-- Silinmiş psikologların bilgilerini saklar
-- Geçmiş randevularda psikolog bilgisi göstermek için

USE [YasamPsikologDb];
GO

-- Tablo oluştur
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PsychologistArchive')
BEGIN
    CREATE TABLE [dbo].[PsychologistArchive] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [OriginalPsychologistId] INT NOT NULL,
        [FirstName] NVARCHAR(100) NOT NULL,
        [LastName] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(255) NOT NULL,
        [PhoneNumber] NVARCHAR(20) NULL,
        [CalendarColor] NVARCHAR(7) NOT NULL DEFAULT '#3788D8',
        [AutoApproveAppointments] BIT NOT NULL DEFAULT 0,
        [ArchivedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ArchivedReason] NVARCHAR(500) NOT NULL DEFAULT N'Psikolog silindi',
        [ArchivedByUser] NVARCHAR(100) NULL,
        [OriginalCreatedAt] DATETIME2 NOT NULL
    );

    PRINT '✓ PsychologistArchive tablosu oluşturuldu';
END
ELSE
BEGIN
    PRINT 'ℹ PsychologistArchive tablosu zaten mevcut';
END
GO

-- Index ekle (hızlı arama için)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PsychologistArchive_OriginalId')
BEGIN
    CREATE INDEX [IX_PsychologistArchive_OriginalId] 
    ON [dbo].[PsychologistArchive]([OriginalPsychologistId]);
    
    PRINT '✓ Index oluşturuldu';
END
GO

PRINT '';
PRINT '🎉 ARŞİV TABLOSU HAZIR!';
PRINT 'Artık psikolog silindiğinde:';
PRINT '  - Bilgileri arşive kopyalanacak';
PRINT '  - Geçmiş randevularda görünecek';
PRINT '  - Veri kaybı olmayacak';
GO
