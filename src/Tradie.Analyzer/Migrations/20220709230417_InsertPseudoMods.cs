using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradie.Analyzer.Migrations
{
    public partial class InsertPseudoMods : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Kind",
                table: "Modifiers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Modifiers",
                columns: new[] { "Id", "Kind", "ModHash", "ModifierText" },
                values: new object[,]
                {
                    { 1327995692, 1, 13132978022138495920m, "+# Total Elemental Resistances" },
                    { 1433901088, 1, 17133152806627075035m, "+# Total Maximum Life" },
                    { 1708230221, 1, 38565874988879569m, "+# Total Resistances" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Modifiers",
                keyColumn: "Id",
                keyValue: 1327995692);

            migrationBuilder.DeleteData(
                table: "Modifiers",
                keyColumn: "Id",
                keyValue: 1433901088);

            migrationBuilder.DeleteData(
                table: "Modifiers",
                keyColumn: "Id",
                keyValue: 1708230221);

            migrationBuilder.DropColumn(
                name: "Kind",
                table: "Modifiers");
        }
    }
}
