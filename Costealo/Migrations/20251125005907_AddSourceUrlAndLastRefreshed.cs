using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Costealo.Migrations
{
    /// <inheritdoc />
    public partial class AddSourceUrlAndLastRefreshed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Use raw SQL with IF NOT EXISTS to make this migration idempotent
            // This allows it to run even if some columns already exist in Azure
            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'FotoPerfil')
                BEGIN
                    ALTER TABLE [Users] ADD [FotoPerfil] nvarchar(max) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'TarjetaCodigoSeguridad')
                BEGIN
                    ALTER TABLE [Users] ADD [TarjetaCodigoSeguridad] nvarchar(max) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'TarjetaFechaVencimiento')
                BEGIN
                    ALTER TABLE [Users] ADD [TarjetaFechaVencimiento] nvarchar(max) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'TarjetaUltimos4Digitos')
                BEGIN
                    ALTER TABLE [Users] ADD [TarjetaUltimos4Digitos] nvarchar(max) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'TipoSuscripcion')
                BEGIN
                    ALTER TABLE [Users] ADD [TipoSuscripcion] nvarchar(max) NOT NULL DEFAULT '';
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'TipoUsuario')
                BEGIN
                    ALTER TABLE [Users] ADD [TipoUsuario] nvarchar(max) NOT NULL DEFAULT '';
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PriceDatabases' AND COLUMN_NAME = 'LastRefreshedAt')
                BEGIN
                    ALTER TABLE [PriceDatabases] ADD [LastRefreshedAt] datetime2 NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PriceDatabases' AND COLUMN_NAME = 'SourceUrl')
                BEGIN
                    ALTER TABLE [PriceDatabases] ADD [SourceUrl] nvarchar(max) NULL;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoPerfil",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TarjetaCodigoSeguridad",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TarjetaFechaVencimiento",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TarjetaUltimos4Digitos",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TipoSuscripcion",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TipoUsuario",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastRefreshedAt",
                table: "PriceDatabases");

            migrationBuilder.DropColumn(
                name: "SourceUrl",
                table: "PriceDatabases");
        }
    }
}
