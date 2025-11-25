-- ===============================================
-- DIAGNÓSTICO COMPLETO DE BASE DE DATOS AZURE
-- Ejecuta esto PRIMERO para ver el estado real
-- ===============================================

PRINT '========================================='
PRINT 'DIAGNÓSTICO COMPLETO - Azure SQL'
PRINT '========================================='
PRINT ''

-- 1. VERIFICAR MIGRACIONES REGISTRADAS
PRINT '1. MIGRACIONES REGISTRADAS:'
PRINT '--------------------------'
SELECT [MigrationId], [ProductVersion] 
FROM [__EFMigrationsHistory] 
ORDER BY [MigrationId];
PRINT ''

-- 2. VERIFICAR TODAS LAS TABLAS
PRINT '2. TABLAS EXISTENTES:'
PRINT '--------------------'
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE' 
ORDER BY TABLE_NAME;
PRINT ''

-- 3. COLUMNAS EN PriceDatabases
PRINT '3. COLUMNAS EN PriceDatabases:'
PRINT '------------------------------'
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'PriceDatabases'
ORDER BY ORDINAL_POSITION;
PRINT ''

-- Verificar columnas críticas
PRINT 'Verificación específica PriceDatabases:'
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PriceDatabases' AND COLUMN_NAME = 'SourceUrl')
    PRINT '  ✅ SourceUrl EXISTS'
ELSE
    PRINT '  ❌ SourceUrl MISSING'

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PriceDatabases' AND COLUMN_NAME = 'LastRefreshedAt')
    PRINT '  ✅ LastRefreshedAt EXISTS'
ELSE
    PRINT '  ❌ LastRefreshedAt MISSING'
PRINT ''

-- 4. COLUMNAS EN Users
PRINT '4. COLUMNAS EN Users:'
PRINT '--------------------'
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Users'
ORDER BY ORDINAL_POSITION;
PRINT ''

-- Verificar columnas críticas
PRINT 'Verificación específica Users:'
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'TipoUsuario')
    PRINT '  ✅ TipoUsuario EXISTS'
ELSE
    PRINT '  ❌ TipoUsuario MISSING'

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'TipoSuscripcion')
    PRINT '  ✅ TipoSuscripcion EXISTS'
ELSE
    PRINT '  ❌ TipoSuscripcion MISSING'

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'FotoPerfil')
    PRINT '  ✅ FotoPerfil EXISTS'
ELSE
    PRINT '  ❌ FotoPerfil MISSING'
PRINT ''

-- 5. COLUMNAS EN PriceItems
PRINT '5. COLUMNAS EN PriceItems:'
PRINT '-------------------------'
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'PriceItems'
ORDER BY ORDINAL_POSITION;
PRINT ''

-- 6. CONTAR REGISTROS
PRINT '6. REGISTROS ACTUALES:'
PRINT '---------------------'
SELECT 'Users' as Tabla, COUNT(*) as Total FROM Users
UNION ALL
SELECT 'PriceDatabases', COUNT(*) FROM PriceDatabases
UNION ALL
SELECT 'PriceItems', COUNT(*) FROM PriceItems
UNION ALL
SELECT 'Workbooks', COUNT(*) FROM Workbooks
UNION ALL
SELECT 'WorkbookItems', COUNT(*) FROM WorkbookItems;
PRINT ''

PRINT '========================================='
PRINT 'FIN DEL DIAGNÓSTICO'
PRINT '========================================='
