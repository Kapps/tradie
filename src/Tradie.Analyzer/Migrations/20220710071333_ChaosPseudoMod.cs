using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradie.Analyzer.Migrations
{
    public partial class ChaosPseudoMod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Modifiers",
                columns: new[] { "Id", "Kind", "ModHash", "ModifierText" },
                values: new object[] { 1890204000, 1, 2780892169640653766m, "+# Total Chaos Resistance" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Modifiers",
                keyColumn: "Id",
                keyValue: 1890204000);
        }
    }
}
