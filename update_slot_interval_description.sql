-- SlotInterval ayarının açıklamasını kullanıcı dostu hale getir
UPDATE SystemSettings
SET Description = N'Randevu saatlerinin kaçar dakikalık aralıklarla gösterileceğini belirler (örn: 5 dakika seçilirse 09:00, 09:05, 09:10 şeklinde)'
WHERE [Key] = 'SlotInterval'
GO
