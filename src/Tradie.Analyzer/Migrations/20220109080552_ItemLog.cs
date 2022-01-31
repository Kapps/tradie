using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Tradie.Analyzer.Migrations
{
    public partial class ItemLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RawId = table.Column<string>(type: "text", nullable: false),
                    StashTabId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Properties = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StashTabs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RawId = table.Column<string>(type: "text", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Owner = table.Column<string>(type: "text", nullable: false),
                    LastCharacterName = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    League = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StashTabs", x => x.Id);
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

            migrationBuilder.CreateIndex(
                name: "idx_stash_Created",
                table: "StashTabs",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "idx_stash_LastModified",
                table: "StashTabs",
                column: "LastModified");

            migrationBuilder.CreateIndex(
                name: "idx_stash_League",
                table: "StashTabs",
                column: "League",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_stash_RawId",
                table: "StashTabs",
                column: "RawId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "StashTabs");
        }
    }
}
