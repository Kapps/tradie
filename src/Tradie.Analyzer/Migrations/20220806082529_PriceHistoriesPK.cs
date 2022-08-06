using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradie.Analyzer.Migrations
{
    public partial class PriceHistoriesPK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemPriceHistory",
                table: "ItemPriceHistory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemPriceHistory",
                table: "ItemPriceHistory",
                columns: new[] { "ItemId", "RecordedTime" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemPriceHistory",
                table: "ItemPriceHistory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemPriceHistory",
                table: "ItemPriceHistory",
                column: "ItemId");
        }
    }
}
