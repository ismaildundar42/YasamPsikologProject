-- SlotInterval ayarını ekle
IF NOT EXISTS (SELECT 1 FROM SystemSettings WHERE [Key] = 'SlotInterval')
BEGIN
    INSERT INTO SystemSettings ([Key], Value, Category, Description, CreatedAt, IsDeleted, IsActive)
    VALUES ('SlotInterval', '5', 'Appointment', 'Randevu slot aralığı (dakika)', GETDATE(), 0, 1)
    PRINT 'SlotInterval ayarı eklendi'
END
ELSE
BEGIN
    PRINT 'SlotInterval ayarı zaten mevcut'
END
GO
