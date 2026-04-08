-- 1. Додаємо колонки, які створив твій колега
ALTER TABLE [Users] ADD [ResetToken] nvarchar(max) NULL;
ALTER TABLE [Users] ADD [ResetTokenExpiry] datetime2 NULL;
GO

-- 2. Кажемо Entity Framework, що ти застосував цю міграцію (під .NET 10)
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES ('20260408190552_AddResetTokenToUser', '10.0.0');
GO