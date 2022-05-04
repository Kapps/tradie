using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Tradie.Analyzer;
using Tradie.Analyzer.Entities;

#nullable disable

namespace Tradie.Analyzer.Migrations
{
    public partial class SeparateLoggedItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_tab_Items",
                table: "StashTabs");

            migrationBuilder.DropColumn(
                name: "Items",
                table: "StashTabs");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PackedItems",
                table: "StashTabs",
                type: "bytea",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "bytea");

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    IdHash = table.Column<long>(type: "bigint", nullable: false),
                    RawId = table.Column<string>(type: "text", nullable: false),
                    RawStashTabId = table.Column<string>(type: "text", nullable: false),
                    Properties = table.Column<Dictionary<ushort, IAnalyzedProperties>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.IdHash);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PackedItems",
                table: "StashTabs",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldNullable: true);

            migrationBuilder.AddColumn<LoggedItem[]>(
                name: "Items",
                table: "StashTabs",
                type: "jsonb",
                nullable: false,
                defaultValue: new LoggedItem[0]);

            migrationBuilder.CreateIndex(
                name: "idx_tab_Items",
                table: "StashTabs",
                column: "Items")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }
    }
}
