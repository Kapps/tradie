using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradie.Analyzer.Migrations
{
    public partial class RemoveModKind : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kind",
                table: "Modifiers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Kind",
                table: "Modifiers",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
