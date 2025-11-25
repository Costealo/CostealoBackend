-- ===============================================
-- SCRIPT DE VERIFICACIÓN POST-MIGRACIÓN
-- Ejecutar DESPUÉS de aplicar check-migration.sql
-- ===============================================

-- 1. Verificar que la migración se registró correctamente
PRINT '========== VERIFICANDO MIGRACIONES =========='
SELECT [MigrationId], [ProductVersion] 
FROM [__EFMigrationsHistory] 
ORDER BY [MigrationId];
-- Debe mostrar ambas migraciones:
-- 20251114014032_Initial
-- 20251125005907_AddSourceUrlAndLastRefreshed
GO

-- 2. Verificar estructura de tabla PriceDatabases
PRINT ''
PRINT '========== COLUMNAS EN PriceDatabases =========='
SELECT 
    COLUMN_NAME as 'Columna',
    DATA_TYPE as 'Tipo',
    IS_NULLABLE as 'Nullable',
    CHARACTER_MAXIMUM_LENGTH as 'Max Length'
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'PriceDatabases'
ORDER BY ORDINAL_POSITION;
-- Debe incluir: SourceUrl, LastRefreshedAt
GO

-- 3. Verificar estructura de tabla Users
PRINT ''
PRINT '========== COLUMNAS EN Users =========='
SELECT 
    COLUMN_NAME as 'Columna',
    DATA_TYPE as 'Tipo',
    IS_NULLABLE as 'Nullable'
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Users'
ORDER BY ORDINAL_POSITION;
-- Debe incluir: TipoUsuario, TipoSuscripcion, FotoPerfil, 
--               TarjetaUltimos4Digitos, TarjetaCodigoSeguridad, TarjetaFechaVencimiento
GO

-- 4. Contar registros actuales (antes de la importación)
PRINT ''
PRINT '========== ESTADO ACTUAL DE DATOS =========='
SELECT 'Users' as Tabla, COUNT(*) as Total FROM Users
UNION ALL
SELECT 'PriceDatabases', COUNT(*) FROM PriceDatabases
UNION ALL
SELECT 'PriceItems', COUNT(*) FROM PriceItems
UNION ALL
SELECT 'Workbooks', COUNT(*) FROM Workbooks
UNION ALL
SELECT 'WorkbookItems', COUNT(*) FROM WorkbookItems;
GO

-- 5. Verificar columnas críticas de PriceDatabases
PRINT ''
PRINT '========== VERIFICACIÓN ESPECÍFICA - PriceDatabases =========='
IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'PriceDatabases' AND COLUMN_NAME = 'SourceUrl'
)
    PRINT '✅ Columna SourceUrl existe'
ELSE
    PRINT '❌ ERROR: Columna SourceUrl NO existe'

IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'PriceDatabases' AND COLUMN_NAME = 'LastRefreshedAt'
)
    PRINT '✅ Columna LastRefreshedAt existe'
ELSE
    PRINT '❌ ERROR: Columna LastRefreshedAt NO existe'
GO

-- 6. Verificar columnas críticas de Users
PRINT ''
PRINT '========== VERIFICACIÓN ESPECÍFICA - Users =========='
IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'TipoUsuario'
)
    PRINT '✅ Columna TipoUsuario existe'
ELSE
    PRINT '❌ ERROR: Columna TipoUsuario NO existe'

IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'FotoPerfil'
)
    PRINT '✅ Columna FotoPerfil existe'
ELSE
    PRINT '❌ ERROR: Columna FotoPerfil NO existe'
GO

PRINT ''
PRINT '========== VERIFICACIÓN COMPLETA =========='
