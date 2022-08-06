using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Tradie.Analyzer.Migrations
{
    public partial class PriceHistoriesSortAndIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PriceHistory",
                table: "PriceHistory");

            migrationBuilder.DropIndex(
                name: "IX_PriceHistory_RecordedTime",
                table: "PriceHistory");

            migrationBuilder.RenameTable(
                name: "PriceHistory",
                newName: "ItemPriceHistory");

            migrationBuilder.RenameIndex(
                name: "IX_PriceHistory_ItemId",
                table: "ItemPriceHistory",
                newName: "IX_ItemPriceHistory_ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemPriceHistory",
                table: "ItemPriceHistory",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPriceHistory_RecordedTime",
                table: "ItemPriceHistory",
                column: "RecordedTime")
                .Annotation("Npgsql:IndexSortOrder", new[] { SortOrder.Descending });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemPriceHistory",
                table: "ItemPriceHistory");

            migrationBuilder.DropIndex(
                name: "IX_ItemPriceHistory_RecordedTime",
                table: "ItemPriceHistory");

            migrationBuilder.RenameTable(
                name: "ItemPriceHistory",
                newName: "PriceHistory");

            migrationBuilder.RenameIndex(
                name: "IX_ItemPriceHistory_ItemId",
                table: "PriceHistory",
                newName: "IX_PriceHistory_ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PriceHistory",
                table: "PriceHistory",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceHistory_RecordedTime",
                table: "PriceHistory",
                column: "RecordedTime");
        }
    }
}
