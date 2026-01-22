-- VERİTABANI FOREIGN KEY'LERİNİ ESKI HALİNE GETİR
-- Bu script tüm değişiklikleri geri alır

USE [YasamPsikologDb];
GO

-- 1. Appointments - Psychologist ilişkisi (RESTRICT)
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Appointments_Psychologists_PsychologistId')
BEGIN
    ALTER TABLE [dbo].[Appointments]
    DROP CONSTRAINT [FK_Appointments_Psychologists_PsychologistId];
    
    ALTER TABLE [dbo].[Appointments]
    ADD CONSTRAINT [FK_Appointments_Psychologists_PsychologistId]
    FOREIGN KEY ([PsychologistId]) REFERENCES [dbo].[Psychologists]([Id])
    ON DELETE NO ACTION;
    
    PRINT 'Appointments - Psychologist FK eski haline getirildi';
END

-- 2. Appointments - Client ilişkisi (RESTRICT)
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Appointments_Clients_ClientId')
BEGIN
    ALTER TABLE [dbo].[Appointments]
    DROP CONSTRAINT [FK_Appointments_Clients_ClientId];
    
    ALTER TABLE [dbo].[Appointments]
    ADD CONSTRAINT [FK_Appointments_Clients_ClientId]
    FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Clients]([Id])
    ON DELETE NO ACTION;
    
    PRINT 'Appointments - Client FK eski haline getirildi';
END

-- 3. Clients - Psychologist ilişkisi (RESTRICT)
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Clients_Psychologists_AssignedPsychologistId')
BEGIN
    ALTER TABLE [dbo].[Clients]
    DROP CONSTRAINT [FK_Clients_Psychologists_AssignedPsychologistId];
    
    ALTER TABLE [dbo].[Clients]
    ADD CONSTRAINT [FK_Clients_Psychologists_AssignedPsychologistId]
    FOREIGN KEY ([AssignedPsychologistId]) REFERENCES [dbo].[Psychologists]([Id])
    ON DELETE NO ACTION;
    
    PRINT 'Clients - Psychologist FK eski haline getirildi';
END

-- 4. WorkingHours - Psychologist ilişkisi (CASCADE - orijinal)
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_WorkingHours_Psychologists_PsychologistId')
BEGIN
    ALTER TABLE [dbo].[WorkingHours]
    DROP CONSTRAINT [FK_WorkingHours_Psychologists_PsychologistId];
    
    ALTER TABLE [dbo].[WorkingHours]
    ADD CONSTRAINT [FK_WorkingHours_Psychologists_PsychologistId]
    FOREIGN KEY ([PsychologistId]) REFERENCES [dbo].[Psychologists]([Id])
    ON DELETE CASCADE;
    
    PRINT 'WorkingHours - Psychologist FK eski haline getirildi (CASCADE)';
END

-- 5. UnavailableTimes - Psychologist ilişkisi (CASCADE - orijinal)
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UnavailableTimes_Psychologists_PsychologistId')
BEGIN
    ALTER TABLE [dbo].[UnavailableTimes]
    DROP CONSTRAINT [FK_UnavailableTimes_Psychologists_PsychologistId];
    
    ALTER TABLE [dbo].[UnavailableTimes]
    ADD CONSTRAINT [FK_UnavailableTimes_Psychologists_PsychologistId]
    FOREIGN KEY ([PsychologistId]) REFERENCES [dbo].[Psychologists]([Id])
    ON DELETE CASCADE;
    
    PRINT 'UnavailableTimes - Psychologist FK eski haline getirildi (CASCADE)';
END

PRINT 'Tüm foreign key kısıtlamaları orijinal haline getirildi!';
GO
