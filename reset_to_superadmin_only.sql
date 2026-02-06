-- Tüm verileri temizle, sadece superadmin kalsın

-- Önce foreign key ilişkileri olan tabloları temizle
DELETE FROM Appointments;
DELETE FROM UnavailableTimes;
DELETE FROM WorkingHours;
DELETE FROM BreakTimes;
DELETE FROM AuditLogs;

-- Psychologist ve Client'ları sil
DELETE FROM Psychologists;
DELETE FROM Clients;

-- SuperAdmin hariç tüm kullanıcıları sil (Role: 0=SuperAdmin, 1=Psychologist, 2=Client)
DELETE FROM Users WHERE Role != 0;

-- SuperAdmin bilgilerini güncelle
UPDATE Users
SET 
    Email = 'superadmin@gmail.com',
    FirstName = 'Super',
    LastName = 'Admin',
    PhoneNumber = '05555555555',
    PasswordHash = '$2a$11$8qGVxQxH0vZP8mYKJ5zMa.SqYx0RGP7sOaL9pYHQKJpw0E8rQeEJu', -- Admin123
    IsActive = 1,
    IsDeleted = 0,
    CreatedAt = GETDATE(),
    UpdatedAt = GETDATE()
WHERE Role = 0;

PRINT 'Veritabanı temizlendi. Sadece SuperAdmin (superadmin@gmail.com / Admin123) kaldı.'
GO
