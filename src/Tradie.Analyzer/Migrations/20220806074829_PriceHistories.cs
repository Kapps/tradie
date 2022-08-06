using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradie.Analyzer.Migrations
{
    public partial class PriceHistories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_modifier_modtext",
                table: "Modifiers");

            migrationBuilder.CreateTable(
                name: "PriceHistory",
                columns: table => new
                {
                    ItemId = table.Column<string>(type: "text", nullable: false),
                    RecordedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Currency = table.Column<byte>(type: "smallint", nullable: false),
                    Amount = table.Column<float>(type: "real", nullable: false),
                    Kind = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceHistory", x => x.ItemId);
                });

            migrationBuilder.CreateIndex(
                name: "idx_modifier_modtext",
                table: "Modifiers",
                column: "ModifierText");

            migrationBuilder.CreateIndex(
                name: "IX_PriceHistory_ItemId",
                table: "PriceHistory",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceHistory_RecordedTime",
                table: "PriceHistory",
                column: "RecordedTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceHistory");

            migrationBuilder.DropIndex(
                name: "idx_modifier_modtext",
                table: "Modifiers");

            migrationBuilder.CreateIndex(
                name: "idx_modifier_modtext",
                table: "Modifiers",
                column: "ModifierText",
                unique: true);
        }
    }
}
