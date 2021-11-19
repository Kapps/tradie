using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradie.Analyzer.Migrations
{
    public partial class AddRequirements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DexRequirement",
                table: "ItemTypes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IntRequirement",
                table: "ItemTypes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LevelRequirement",
                table: "ItemTypes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StrRequirement",
                table: "ItemTypes",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DexRequirement",
                table: "ItemTypes");

            migrationBuilder.DropColumn(
                name: "IntRequirement",
                table: "ItemTypes");

            migrationBuilder.DropColumn(
                name: "LevelRequirement",
                table: "ItemTypes");

            migrationBuilder.DropColumn(
                name: "StrRequirement",
                table: "ItemTypes");
        }
    }
}
