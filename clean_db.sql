USE YasamPsikologDb;

-- Foreign key kısıtlamalarını geçici olarak kaldır
ALTER TABLE Clients NOCHECK CONSTRAINT ALL;
ALTER TABLE Psychologists NOCHECK CONSTRAINT ALL;
ALTER TABLE Appointments NOCHECK CONSTRAINT ALL;
ALTER TABLE UnavailableTimes NOCHECK CONSTRAINT ALL;
ALTER TABLE WorkingHours NOCHECK CONSTRAINT ALL;

-- Tüm verileri temizle
DELETE FROM Appointments;
DELETE FROM UnavailableTimes;
DELETE FROM WorkingHours;
DELETE FROM Clients;
DELETE FROM Psychologists;
DELETE FROM Users;

-- Foreign key kısıtlamalarını tekrar aktif et
ALTER TABLE Clients CHECK CONSTRAINT ALL;
ALTER TABLE Psychologists CHECK CONSTRAINT ALL;
ALTER TABLE Appointments CHECK CONSTRAINT ALL;
ALTER TABLE UnavailableTimes CHECK CONSTRAINT ALL;
ALTER TABLE WorkingHours CHECK CONSTRAINT ALL;

-- Tek süper admin oluştur
INSERT INTO Users (Email, PasswordHash, FirstName, LastName, PhoneNumber, Role, Gender, IsActive, IsDeleted, CreatedAt)
VALUES ('superadmin@gmail.com', 'Admin123', 'Super', 'Admin', '05000000000', 0, 0, 1, 0, GETDATE());

-- Sonucu kontrol et
SELECT Id, Email, FirstName, LastName, Role FROM Users;
