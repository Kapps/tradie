using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradie.Analyzer.Migrations
{
    public partial class FixLeagueBeingUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_stash_League",
                table: "StashTabs");

            migrationBuilder.CreateIndex(
                name: "idx_stash_League",
                table: "StashTabs",
                column: "League");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_stash_League",
                table: "StashTabs");

            migrationBuilder.CreateIndex(
                name: "idx_stash_League",
                table: "StashTabs",
                column: "League",
                unique: true);
        }
    }
}
