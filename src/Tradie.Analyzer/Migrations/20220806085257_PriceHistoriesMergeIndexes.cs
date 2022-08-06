using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Tradie.Analyzer.Migrations
{
    public partial class PriceHistoriesMergeIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ItemPriceHistory_RecordedTime",
                table: "ItemPriceHistory");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPriceHistory_ItemId_RecordedTime",
                table: "ItemPriceHistory",
                columns: new[] { "ItemId", "RecordedTime" })
                .Annotation("Npgsql:IndexSortOrder", new[] { SortOrder.Ascending, SortOrder.Descending });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ItemPriceHistory_ItemId_RecordedTime",
                table: "ItemPriceHistory");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPriceHistory_RecordedTime",
                table: "ItemPriceHistory",
                column: "RecordedTime")
                .Annotation("Npgsql:IndexSortOrder", new[] { SortOrder.Descending });
        }
    }
}
