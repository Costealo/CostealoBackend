-- ===============================================
-- SOLUCIÓN LIMPIA Y DEFINITIVA
-- Solo agrega lo que falta: PriceDatabases.LastRefreshedAt
-- ===============================================

BEGIN TRANSACTION;

PRINT '========== APLICANDO SOLUCIÓN LIMPIA =========='
PRINT ''

-- PASO 1: Agregar la columna que falta
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'PriceDatabases' AND COLUMN_NAME = 'LastRefreshedAt'
)
BEGIN
    ALTER TABLE [PriceDatabases] ADD [LastRefreshedAt] datetime2 NULL;
    PRINT '✅ Columna LastRefreshedAt agregada a PriceDatabases'
END
ELSE
BEGIN
    PRINT '⚠️ LastRefreshedAt ya existe (esto es bueno)'
END

PRINT ''

-- PASO 2: Registrar la migración como aplicada
IF NOT EXISTS (
    SELECT 1 FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125005907_AddSourceUrlAndLastRefreshed'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251125005907_AddSourceUrlAndLastRefreshed', N'9.0.9');
    PRINT '✅ Migración registrada en __EFMigrationsHistory'
END
ELSE
BEGIN
    PRINT '⚠️ Migración ya está registrada'
END

COMMIT;

PRINT ''
PRINT '========== VERIFICACIÓN FINAL =========='

-- Verificar que LastRefreshedAt existe
IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'PriceDatabases' AND COLUMN_NAME = 'LastRefreshedAt'
)
    PRINT '✅ CONFIRMADO: PriceDatabases.LastRefreshedAt existe'
ELSE
    PRINT '❌ ERROR: LastRefreshedAt NO existe'

-- Verificar que la migración está registrada
IF EXISTS (
    SELECT 1 FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125005907_AddSourceUrlAndLastRefreshed'
)
    PRINT '✅ CONFIRMADO: Migración registrada'
ELSE
    PRINT '❌ ERROR: Migración NO registrada'

PRINT ''
PRINT '========== LISTO =========='
PRINT 'Ahora puedes importar Excel/URL y los datos se guardarán correctamente.'
