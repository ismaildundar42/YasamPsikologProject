-- Veritabanını temizle - Sadece SuperAdmin kalsın
USE YasamPsikologDb;
GO

-- Tüm randevuları sil
DELETE FROM Appointments;
GO

-- Tüm unavailable times'ları sil
DELETE FROM UnavailableTimes;
GO

-- Tüm çalışma saatlerini sil
DELETE FROM WorkingHours;
GO

-- Tüm mola saatlerini sil
DELETE FROM BreakTimes;
GO

-- SuperAdmin dışındaki tüm kullanıcıları sil
DELETE FROM Users WHERE Role != 0;
GO

-- SuperAdmin dışındaki psychologist kayıtlarını sil
DELETE FROM Psychologists WHERE UserId NOT IN (SELECT Id FROM Users WHERE Role = 0);
GO

-- SuperAdmin dışındaki client kayıtlarını sil
DELETE FROM Clients WHERE UserId NOT IN (SELECT Id FROM Users WHERE Role = 0);
GO

PRINT 'Veritabanı temizlendi. Sadece SuperAdmin kaldı.';
GO
