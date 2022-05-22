using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradie.Analyzer.Migrations
{
    public partial class AffixRange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string[]>(
                name: "Subcategories",
                table: "ItemTypes",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(string[]),
                oldType: "text[]");

            migrationBuilder.AlterColumn<int>(
                name: "StrRequirement",
                table: "ItemTypes",
                type: "integer",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "LevelRequirement",
                table: "ItemTypes",
                type: "integer",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "IntRequirement",
                table: "ItemTypes",
                type: "integer",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "IconUrl",
                table: "ItemTypes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "DexRequirement",
                table: "ItemTypes",
                type: "integer",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AffixRanges",
                columns: table => new
                {
                    ModHash = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    EntityKind = table.Column<byte>(type: "smallint", nullable: false),
                    ModCategory = table.Column<byte>(type: "smallint", nullable: false),
                    MinValue = table.Column<float>(type: "real", nullable: true),
                    MaxValue = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffixRanges", x => new { x.ModHash, x.ModCategory, x.EntityKind });
                });

            migrationBuilder.CreateIndex(
                name: "IX_AffixRanges_EntityKind",
                table: "AffixRanges",
                column: "EntityKind");

            migrationBuilder.CreateIndex(
                name: "IX_AffixRanges_ModCategory",
                table: "AffixRanges",
                column: "ModCategory");

            migrationBuilder.CreateIndex(
                name: "IX_AffixRanges_ModHash",
                table: "AffixRanges",
                column: "ModHash");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AffixRanges");

            migrationBuilder.AlterColumn<string[]>(
                name: "Subcategories",
                table: "ItemTypes",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StrRequirement",
                table: "ItemTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true,
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "LevelRequirement",
                table: "ItemTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true,
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "IntRequirement",
                table: "ItemTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true,
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "IconUrl",
                table: "ItemTypes",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DexRequirement",
                table: "ItemTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true,
                oldDefaultValue: 0);
        }
    }
}
