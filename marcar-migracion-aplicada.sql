-- ===============================================
-- SOLUCIÓN: Marcar migración como aplicada
-- Las columnas YA EXISTEN pero no se registró la migración
-- ===============================================

-- Solo insertar el registro de la migración SIN ejecutar los ALTER TABLE
BEGIN TRANSACTION;

-- Verificar que la migración NO está registrada
IF NOT EXISTS (
    SELECT 1 FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125005907_AddSourceUrlAndLastRefreshed'
)
BEGIN
    PRINT '✅ Insertando registro de migración...'
    
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251125005907_AddSourceUrlAndLastRefreshed', N'9.0.9');
    
    PRINT '✅ Migración marcada como aplicada'
END
ELSE
BEGIN
    PRINT '⚠️ La migración ya está registrada'
END

COMMIT;
GO

-- Verificar el resultado
PRINT ''
PRINT '========== MIGRACIONES REGISTRADAS =========='
SELECT [MigrationId], [ProductVersion] 
FROM [__EFMigrationsHistory] 
ORDER BY [MigrationId];
GO
