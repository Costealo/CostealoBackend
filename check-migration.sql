BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125005907_AddSourceUrlAndLastRefreshed'
)
BEGIN
    ALTER TABLE [Users] ADD [FotoPerfil] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125005907_AddSourceUrlAndLastRefreshed'
)
BEGIN
    ALTER TABLE [Users] ADD [TarjetaCodigoSeguridad] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125005907_AddSourceUrlAndLastRefreshed'
)
BEGIN
    ALTER TABLE [Users] ADD [TarjetaFechaVencimiento] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125005907_AddSourceUrlAndLastRefreshed'
)
BEGIN
    ALTER TABLE [Users] ADD [TarjetaUltimos4Digitos] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125005907_AddSourceUrlAndLastRefreshed'
)
BEGIN
    ALTER TABLE [Users] ADD [TipoSuscripcion] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125005907_AddSourceUrlAndLastRefreshed'
)
BEGIN
    ALTER TABLE [Users] ADD [TipoUsuario] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125005907_AddSourceUrlAndLastRefreshed'
)
BEGIN
    ALTER TABLE [PriceDatabases] ADD [LastRefreshedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125005907_AddSourceUrlAndLastRefreshed'
)
BEGIN
    ALTER TABLE [PriceDatabases] ADD [SourceUrl] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125005907_AddSourceUrlAndLastRefreshed'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251125005907_AddSourceUrlAndLastRefreshed', N'9.0.9');
END;

COMMIT;
GO

