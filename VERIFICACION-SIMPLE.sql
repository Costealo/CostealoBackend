-- ===============================================
-- VERIFICACIÓN SIMPLE Y CLARA
-- Solo las columnas que necesitamos verificar
-- ===============================================

PRINT '========== ESTADO ACTUAL DE AZURE SQL =========='
PRINT ''

-- 1. ¿Qué migración está registrada?
PRINT '1. MIGRACIONES REGISTRADAS:'
SELECT [MigrationId] FROM [__EFMigrationsHistory] ORDER BY [MigrationId];
PRINT ''

-- 2. Columnas en PriceDatabases (TODAS)
PRINT '2. TODAS LAS COLUMNAS EN PriceDatabases:'
SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'PriceDatabases'
ORDER BY ORDINAL_POSITION;
PRINT ''

-- 3. Columnas en Users (TODAS)
PRINT '3. TODAS LAS COLUMNAS EN Users:'
SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Users'
ORDER BY ORDINAL_POSITION;
PRINT ''

-- 4. RESUMEN DE LO QUE DEBE EXISTIR vs LO QUE EXISTE
PRINT '========== ANÁLISIS DE DISCREPANCIAS =========='
PRINT ''
PRINT 'COLUMNAS QUE DEBE TENER PriceDatabases según el código:'
PRINT '  - Id'
PRINT '  - UserId'
PRINT '  - Nombre'
PRINT '  - BlobUrl'
PRINT '  - FileName'
PRINT '  - FileType'
PRINT '  - RowCount'
PRINT '  - IsValidated'
PRINT '  - CreatedAt'
PRINT '  - SourceUrl        ← Verificar'
PRINT '  - LastRefreshedAt  ← Verificar'
PRINT ''

PRINT 'COLUMNAS QUE DEBE TENER Users según el código:'
PRINT '  - Id'
PRINT '  - Nombre'
PRINT '  - Email'
PRINT '  - PasswordHash'
PRINT '  - Role'
PRINT '  - TipoUsuario         ← Verificar'
PRINT '  - TipoSuscripcion     ← Verificar'
PRINT '  - FotoPerfil          ← Verificar'
PRINT '  - TarjetaUltimos4Digitos    ← Verificar'
PRINT '  - TarjetaCodigoSeguridad    ← Verificar'
PRINT '  - TarjetaFechaVencimiento   ← Verificar'
PRINT ''

-- 5. Verificación específica
PRINT 'VERIFICACIÓN (lo que falta agregar):'
PRINT '-------------------------------------------'

-- PriceDatabases.SourceUrl
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PriceDatabases' AND COLUMN_NAME = 'SourceUrl')
    PRINT '❌ FALTA: PriceDatabases.SourceUrl'
ELSE
    PRINT '✅ EXISTE: PriceDatabases.SourceUrl'

-- PriceDatabases.LastRefreshedAt
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PriceDatabases' AND COLUMN_NAME = 'LastRefreshedAt')
    PRINT '❌ FALTA: PriceDatabases.LastRefreshedAt'
ELSE
    PRINT '✅ EXISTE: PriceDatabases.LastRefreshedAt'

-- Users.TipoUsuario
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'TipoUsuario')
    PRINT '❌ FALTA: Users.TipoUsuario'
ELSE
    PRINT '✅ EXISTE: Users.TipoUsuario'

-- Users.TipoSuscripcion
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'TipoSuscripcion')
    PRINT '❌ FALTA: Users.TipoSuscripcion'
ELSE
    PRINT '✅ EXISTE: Users.TipoSuscripcion'

-- Users.FotoPerfil
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'FotoPerfil')
    PRINT '❌ FALTA: Users.FotoPerfil'
ELSE
    PRINT '✅ EXISTE: Users.FotoPerfil'

-- Users.TarjetaUltimos4Digitos
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'TarjetaUltimos4Digitos')
    PRINT '❌ FALTA: Users.TarjetaUltimos4Digitos'
ELSE
    PRINT '✅ EXISTE: Users.TarjetaUltimos4Digitos'

-- Users.TarjetaCodigoSeguridad
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'TarjetaCodigoSeguridad')
    PRINT '❌ FALTA: Users.TarjetaCodigoSeguridad'
ELSE
    PRINT '✅ EXISTE: Users.TarjetaCodigoSeguridad'

-- Users.TarjetaFechaVencimiento
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'TarjetaFechaVencimiento')
    PRINT '❌ FALTA: Users.TarjetaFechaVencimiento'
ELSE
    PRINT '✅ EXISTE: Users.TarjetaFechaVencimiento'

PRINT ''
PRINT '========== FIN VERIFICACIÓN =========='
