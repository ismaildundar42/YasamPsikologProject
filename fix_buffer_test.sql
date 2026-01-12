-- SADECE TEST İÇİN: Tüm randevuları sil ve temiz başla
USE YasamPsikologDb;
GO

-- Tüm randevuları sil
DELETE FROM Appointments;

-- Tüm çalışma saatlerindeki buffer'ı 15 dakika yap
UPDATE WorkingHours SET BufferDuration = 15;

-- Kontrol et
SELECT Id, PsychologistId, DayOfWeek, StartTime, EndTime, BufferDuration
FROM WorkingHours;

SELECT Id, PsychologistId, AppointmentDate, Duration, BreakDuration, AppointmentEndDate, Status
FROM Appointments;

PRINT 'Temizlendi! Şimdi uygulamayı başlat ve 09:00 da 90 dakikalık randevu al.';
PRINT 'Sonraki randevu 10:45 te gösterilmeli!';
