using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Tradie.Analyzer.Entities;

#nullable disable

namespace Tradie.Analyzer.Migrations
{
    public partial class LoggedItemsWithinTabDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.AddColumn<LoggedItem[]>(
                name: "Items",
                table: "StashTabs",
                type: "jsonb",
                nullable: false,
                defaultValue: new LoggedItem[0]);

            migrationBuilder.CreateIndex(
                name: "idx_tab_Items",
                table: "StashTabs",
                column: "Items")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_tab_Items",
                table: "StashTabs");

            migrationBuilder.DropColumn(
                name: "Items",
                table: "StashTabs");

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Properties = table.Column<byte[]>(type: "jsonb", nullable: false),
                    RawId = table.Column<string>(type: "text", nullable: false),
                    StashTabId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_item_Properties",
                table: "Items",
                column: "Properties")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "idx_item_RawId",
                table: "Items",
                column: "RawId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_item_StashTabId",
                table: "Items",
                column: "StashTabId");
        }
    }
}
