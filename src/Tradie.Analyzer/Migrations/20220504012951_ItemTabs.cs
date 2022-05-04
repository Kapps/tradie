using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradie.Analyzer.Migrations
{
    public partial class ItemTabs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RawStashTabId",
                table: "Items");

            migrationBuilder.AddColumn<long>(
                name: "StashTabId",
                table: "Items",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Items_IdHash",
                table: "Items",
                column: "IdHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_RawId",
                table: "Items",
                column: "RawId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_StashTabId",
                table: "Items",
                column: "StashTabId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Items_IdHash",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_RawId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_StashTabId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "StashTabId",
                table: "Items");

            migrationBuilder.AddColumn<string>(
                name: "RawStashTabId",
                table: "Items",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
