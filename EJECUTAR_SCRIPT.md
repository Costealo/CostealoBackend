# 🚀 Ejecutar Script de Migración en Azure SQL

Tienes **3 opciones** para ejecutar el script `check-migration.sql` en Azure:

---

## ✅ OPCIÓN 1: Azure Portal (RECOMENDADO - Más Fácil)

### Pasos:
1. Ir a https://portal.azure.com
2. Buscar tu SQL Database: `costealo-db`
3. En el menú lateral, ir a **"Query editor (preview)"**
4. Login con:
   - **Usuario**: `costealo`
   - **Contraseña**: `PasswOrd3`
5. Copiar y pegar TODO el contenido de `check-migration.sql`
6. Click en **"Run"**
7. ✅ Debería ejecutarse sin errores y mostrar "Query succeeded"

---

## ✅ OPCIÓN 2: Visual Studio (Si estás trabajando allí)

### Pasos:
1. En Visual Studio, ir a **View > SQL Server Object Explorer**
2. Expandir **SQL Server > Azure**
3. Conectar a:
   - Server: `costealo-srv.database.windows.net`
   - Database: `costealo-db`
   - User: `costealo`
   - Password: `PasswOrd3`
4. Right-click en `costealo-db` > **New Query**
5. Pegar el contenido de `check-migration.sql`
6. Click en **Execute** (botón verde)

---

## ✅ OPCIÓN 3: Azure Data Studio (Si lo tienes instalado)

### Pasos:
1. Abrir Azure Data Studio
2. Click en **"New Connection"**
3. Configurar:
   - **Server**: `costealo-srv.database.windows.net`
   - **Database**: `costealo-db`
   - **Authentication type**: SQL Login
   - **User name**: `costealo`
   - **Password**: `PasswOrd3`
4. Click **Connect**
5. Click en **"New Query"**
6. Abrir el archivo `check-migration.sql` o copiar su contenido
7. Click en **"Run"** o presionar `F5`

---

## 🔍 Verificación Post-Ejecución

Después de ejecutar el script, verificar que funcionó:

### Desde cualquier Query Editor en Azure:
```sql
-- Verificar que la migración se registró
SELECT * FROM __EFMigrationsHistory ORDER BY MigrationId;

-- Debería mostrar:
-- 20251114014032_Initial
-- 20251125005907_AddSourceUrlAndLastRefreshed  ← Esta es la nueva

-- Verificar columnas en PriceDatabases
SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'PriceDatabases'
ORDER BY ORDINAL_POSITION;

-- Debería incluir: SourceUrl, LastRefreshedAt

-- Verificar columnas en Users
SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Users'
ORDER BY ORDINAL_POSITION;

-- Debería incluir: TipoUsuario, TipoSuscripcion, FotoPerfil, etc.
```

---

## 📍 Contenido del Script (check-migration.sql)

**Ubicación**: `c:\Users\pitu0\RiderProjects\CostealoBackend\check-migration.sql`

El script es **idempotent** (seguro para ejecutar múltiples veces) porque:
- ✅ Usa `IF NOT EXISTS` antes de cada `ALTER TABLE`
- ✅ Solo agrega columnas si no existen
- ✅ No da error si las columnas ya están presentes

---

## ✨ Resultado Esperado

Después de ejecutar el script exitosamente:
- ✅ 8 columnas nuevas agregadas a Azure SQL
- ✅ Importaciones de Excel/URL se guardarán correctamente
- ✅ Al refrescar la página, los datos persistirán
- ✅ Perfiles de usuario podrán guardar foto y datos de pago

---

## ❓ Si algo sale mal

**Error 1**: "Login failed for user 'costealo'"
- Verificar que el firewall de Azure SQL permite tu IP
- Ir a Azure Portal > SQL Database > Networking > agregar tu IP actual

**Error 2**: "Column names must be unique"
- ✅ No debería pasar porque el script es idempotent
- Si pasa, significa que algunas columnas ya existen → el script las saltará automáticamente

**Error 3**: "Cannot find the object '__EFMigrationsHistory'"
- Significa que nunca se aplicó la migración inicial
- Ejecutar primero: `dotnet ef database update` desde terminal

---

## 🎯 ¿Cuál opción usar?

**Más rápido**: OPCIÓN 1 (Azure Portal) - Solo necesitas navegador  
**Más control**: OPCIÓN 2 (Visual Studio) - Si ya estás trabajando allí  
**Más profesional**: OPCIÓN 3 (Azure Data Studio) - Herramienta dedicada

**Mi recomendación**: OPCIÓN 1 (Azure Portal) porque es la más directa.
