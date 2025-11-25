# Aplicar Migraciones Manualmente a Azure SQL

Ya que las columnas de `User` ya existen pero falta marcar la migración como aplicada, tenemos dos opciones:

## Opción 1: Ejecutar el script SQL idempotent generado (RECOMENDADO)
El script `check-migration.sql` tiene validaciones `IF NOT EXISTS` para cada ALTER TABLE, entonces:
1. Solo agregará las columnas que faltan (SourceUrl y LastRefreshedAt si no existen)
2. No dará error si las columnas ya existen  
3. Registrará la migración en `__EFMigrationsHistory`

**Ejecutar**:
Conectarse a Azure SQL y ejecutar el archivo `check-migration.sql`

## Opción 2: Marcar manualmente la migración como aplicada
Si sabemos que TODAS las columnas ya existen en Azure, podemos solo insertar el registro:

```sql
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251125005907_AddSourceUrlAndLastRefreshed', N'9.0.9');
```

## ⚠️ Problema Real a Investigar

Después de aplicar esta migración, aún necesitamos investigar POR QUÉ los datos no se guardan.
Posibles causas:
1. Las tablas PriceDatabases, PriceItems no existen en Azure
2. Hay errores silenciosos en SaveChangesAsync que no se están reportando
3. El código  backend está usando otra base de datos localmente

## Siguiente Paso
Verificar que las tablas existen en Azure SQL:
```sql
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME;
```
