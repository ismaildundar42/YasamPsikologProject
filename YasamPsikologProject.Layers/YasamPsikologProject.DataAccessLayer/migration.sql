IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE TABLE [SystemSettings] (
        [Id] int NOT NULL IDENTITY,
        [Key] nvarchar(100) NOT NULL,
        [Value] nvarchar(1000) NOT NULL,
        [Description] nvarchar(500) NULL,
        [Category] nvarchar(50) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_SystemSettings] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [FirstName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [Email] nvarchar(255) NOT NULL,
        [PhoneNumber] nvarchar(20) NOT NULL,
        [PasswordHash] nvarchar(255) NOT NULL,
        [Role] int NOT NULL,
        [Gender] int NOT NULL,
        [DateOfBirth] datetime2 NULL,
        [ProfileImageUrl] nvarchar(500) NULL,
        [LastLoginAt] datetime2 NULL,
        [RefreshToken] nvarchar(500) NULL,
        [RefreshTokenExpiryTime] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NULL,
        [Action] nvarchar(50) NOT NULL,
        [EntityName] nvarchar(100) NOT NULL,
        [EntityId] int NULL,
        [Description] nvarchar(1000) NULL,
        [IpAddress] nvarchar(50) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AuditLogs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE TABLE [Psychologists] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [LicenseNumber] nvarchar(50) NOT NULL,
        [Specialization] nvarchar(200) NULL,
        [Biography] nvarchar(1000) NULL,
        [ExperienceYears] int NOT NULL,
        [Education] nvarchar(500) NULL,
        [Certifications] nvarchar(500) NULL,
        [CalendarColor] nvarchar(7) NOT NULL DEFAULT N'#3788D8',
        [IsOnlineConsultationAvailable] bit NOT NULL,
        [IsInPersonConsultationAvailable] bit NOT NULL,
        [AutoApproveAppointments] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Psychologists] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Psychologists_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE TABLE [Clients] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [AssignedPsychologistId] int NULL,
        [Address] nvarchar(500) NULL,
        [Notes] nvarchar(2000) NULL,
        [KvkkConsentGiven] bit NOT NULL DEFAULT CAST(0 AS bit),
        [KvkkConsentDate] datetime2 NULL,
        [PreferredNotificationMethod] int NULL,
        [WhatsAppNotificationEnabled] bit NOT NULL,
        [SmsNotificationEnabled] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Clients] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Clients_Psychologists_AssignedPsychologistId] FOREIGN KEY ([AssignedPsychologistId]) REFERENCES [Psychologists] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Clients_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE TABLE [UnavailableTimes] (
        [Id] int NOT NULL IDENTITY,
        [PsychologistId] int NOT NULL,
        [StartDateTime] datetime2 NOT NULL,
        [EndDateTime] datetime2 NOT NULL,
        [Reason] nvarchar(500) NOT NULL,
        [IsAllDay] bit NOT NULL,
        [Notes] nvarchar(1000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_UnavailableTimes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UnavailableTimes_Psychologists_PsychologistId] FOREIGN KEY ([PsychologistId]) REFERENCES [Psychologists] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE TABLE [WorkingHours] (
        [Id] int NOT NULL IDENTITY,
        [PsychologistId] int NOT NULL,
        [DayOfWeek] int NOT NULL,
        [StartTime] time NOT NULL,
        [EndTime] time NOT NULL,
        [IsAvailable] bit NOT NULL DEFAULT CAST(1 AS bit),
        [BreakStartTime] time NULL,
        [BreakEndTime] time NULL,
        [Notes] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_WorkingHours] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_WorkingHours_Psychologists_PsychologistId] FOREIGN KEY ([PsychologistId]) REFERENCES [Psychologists] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE TABLE [Appointments] (
        [Id] int NOT NULL IDENTITY,
        [ClientId] int NOT NULL,
        [PsychologistId] int NOT NULL,
        [AppointmentDate] datetime2 NOT NULL,
        [Duration] int NOT NULL,
        [AppointmentEndDate] datetime2 NOT NULL,
        [BreakDuration] int NOT NULL DEFAULT 10,
        [Status] int NOT NULL,
        [IsOnline] bit NOT NULL,
        [ClientNotes] nvarchar(1000) NULL,
        [PsychologistNotes] nvarchar(2000) NULL,
        [CancelledAt] datetime2 NULL,
        [CancellationReason] nvarchar(500) NULL,
        [ReminderSent] bit NOT NULL,
        [MeetingLink] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Appointments_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Clients] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Appointments_Psychologists_PsychologistId] FOREIGN KEY ([PsychologistId]) REFERENCES [Psychologists] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE TABLE [AppointmentNotifications] (
        [Id] int NOT NULL IDENTITY,
        [AppointmentId] int NOT NULL,
        [NotificationType] int NOT NULL,
        [RecipientContact] nvarchar(255) NOT NULL,
        [RecipientPhoneNumber] nvarchar(20) NOT NULL,
        [RecipientEmail] nvarchar(255) NOT NULL,
        [Message] nvarchar(2000) NOT NULL,
        [IsSent] bit NOT NULL,
        [SentAt] datetime2 NULL,
        [HasError] bit NOT NULL,
        [ErrorMessage] nvarchar(1000) NULL,
        [NotificationPurpose] nvarchar(50) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_AppointmentNotifications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AppointmentNotifications_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE INDEX [IX_AppointmentNotifications_AppointmentId] ON [AppointmentNotifications] ([AppointmentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE INDEX [IX_AppointmentNotifications_IsSent] ON [AppointmentNotifications] ([IsSent]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE INDEX [IX_Appointments_AppointmentDate] ON [Appointments] ([AppointmentDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE INDEX [IX_Appointments_ClientId] ON [Appointments] ([ClientId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE INDEX [IX_Appointments_Overlap_Check] ON [Appointments] ([PsychologistId], [AppointmentDate], [AppointmentEndDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE INDEX [IX_Appointments_PsychologistId] ON [Appointments] ([PsychologistId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE INDEX [IX_Appointments_Status] ON [Appointments] ([Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_CreatedAt] ON [AuditLogs] ([CreatedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_Entity] ON [AuditLogs] ([EntityName], [EntityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_UserId] ON [AuditLogs] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE INDEX [IX_Clients_AssignedPsychologistId] ON [Clients] ([AssignedPsychologistId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Clients_UserId] ON [Clients] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Psychologists_LicenseNumber] ON [Psychologists] ([LicenseNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Psychologists_UserId] ON [Psychologists] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE INDEX [IX_SystemSettings_Category] ON [SystemSettings] ([Category]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SystemSettings_Key] ON [SystemSettings] ([Key]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE INDEX [IX_UnavailableTimes_DateRange] ON [UnavailableTimes] ([PsychologistId], [StartDateTime], [EndDateTime]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE INDEX [IX_UnavailableTimes_PsychologistId] ON [UnavailableTimes] ([PsychologistId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_PhoneNumber] ON [Users] ([PhoneNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    CREATE INDEX [IX_WorkingHours_Psychologist_Day] ON [WorkingHours] ([PsychologistId], [DayOfWeek]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216190714_mig_first'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251216190714_mig_first', N'8.0.17');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251217112202_mig_second'
)
BEGIN
    ALTER TABLE [Psychologists] ADD [ConsultationDuration] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251217112202_mig_second'
)
BEGIN
    ALTER TABLE [Psychologists] ADD [ConsultationFee] decimal(18,2) NOT NULL DEFAULT 0.0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251217112202_mig_second'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251217112202_mig_second', N'8.0.17');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251217121755_mig_Three'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Psychologists]') AND [c].[name] = N'Specialization');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Psychologists] DROP CONSTRAINT [' + @var0 + '];');
    EXEC(N'UPDATE [Psychologists] SET [Specialization] = N'''' WHERE [Specialization] IS NULL');
    ALTER TABLE [Psychologists] ALTER COLUMN [Specialization] nvarchar(200) NOT NULL;
    ALTER TABLE [Psychologists] ADD DEFAULT N'' FOR [Specialization];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251217121755_mig_Three'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251217121755_mig_Three', N'8.0.17');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260112111656_RemovePsychologistUnnecessaryFields'
)
BEGIN
    DROP INDEX [IX_Psychologists_LicenseNumber] ON [Psychologists];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260112111656_RemovePsychologistUnnecessaryFields'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Psychologists]') AND [c].[name] = N'ConsultationDuration');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Psychologists] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Psychologists] DROP COLUMN [ConsultationDuration];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260112111656_RemovePsychologistUnnecessaryFields'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Psychologists]') AND [c].[name] = N'ConsultationFee');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Psychologists] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [Psychologists] DROP COLUMN [ConsultationFee];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260112111656_RemovePsychologistUnnecessaryFields'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Psychologists]') AND [c].[name] = N'LicenseNumber');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Psychologists] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [Psychologists] DROP COLUMN [LicenseNumber];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260112111656_RemovePsychologistUnnecessaryFields'
)
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Psychologists]') AND [c].[name] = N'Specialization');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Psychologists] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [Psychologists] DROP COLUMN [Specialization];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260112111656_RemovePsychologistUnnecessaryFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260112111656_RemovePsychologistUnnecessaryFields', N'8.0.17');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260112112839_RemovePsychologistProfileFields'
)
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Psychologists]') AND [c].[name] = N'Biography');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Psychologists] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [Psychologists] DROP COLUMN [Biography];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260112112839_RemovePsychologistProfileFields'
)
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Psychologists]') AND [c].[name] = N'Certifications');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Psychologists] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [Psychologists] DROP COLUMN [Certifications];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260112112839_RemovePsychologistProfileFields'
)
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Psychologists]') AND [c].[name] = N'Education');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Psychologists] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [Psychologists] DROP COLUMN [Education];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260112112839_RemovePsychologistProfileFields'
)
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Psychologists]') AND [c].[name] = N'ExperienceYears');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [Psychologists] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [Psychologists] DROP COLUMN [ExperienceYears];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260112112839_RemovePsychologistProfileFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260112112839_RemovePsychologistProfileFields', N'8.0.17');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260112114158_AddBreakTimesTable'
)
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[WorkingHours]') AND [c].[name] = N'BreakEndTime');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [WorkingHours] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [WorkingHours] DROP COLUMN [BreakEndTime];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260112114158_AddBreakTimesTable'
)
BEGIN
    DECLARE @var10 sysname;
    SELECT @var10 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[WorkingHours]') AND [c].[name] = N'BreakStartTime');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [WorkingHours] DROP CONSTRAINT [' + @var10 + '];');
    ALTER TABLE [WorkingHours] DROP COLUMN [BreakStartTime];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260112114158_AddBreakTimesTable'
)
BEGIN
    CREATE TABLE [BreakTimes] (
        [Id] int NOT NULL IDENTITY,
        [WorkingHourId] int NOT NULL,
        [StartTime] time NOT NULL,
        [EndTime] time NOT NULL,
        [Notes] nvarchar(200) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_BreakTimes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BreakTimes_WorkingHours_WorkingHourId] FOREIGN KEY ([WorkingHourId]) REFERENCES [WorkingHours] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260112114158_AddBreakTimesTable'
)
BEGIN
    CREATE INDEX [IX_BreakTimes_WorkingHour] ON [BreakTimes] ([WorkingHourId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260112114158_AddBreakTimesTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260112114158_AddBreakTimesTable', N'8.0.17');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260112120724_AddBufferDurationToWorkingHour'
)
BEGIN
    ALTER TABLE [WorkingHours] ADD [BufferDuration] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260112120724_AddBufferDurationToWorkingHour'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260112120724_AddBufferDurationToWorkingHour', N'8.0.17');
END;
GO

COMMIT;
GO

