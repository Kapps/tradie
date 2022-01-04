using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Tradie.Analyzer.Migrations
{
    public partial class AddModifiers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Modifiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kind = table.Column<byte>(type: "smallint", nullable: false),
                    ModHash = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    ModifierText = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modifiers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_itemtype_category",
                table: "ItemTypes",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "idx_itemtype_subcategory",
                table: "ItemTypes",
                column: "Subcategory");

            migrationBuilder.CreateIndex(
                name: "idx_modifier_modhash",
                table: "Modifiers",
                column: "ModHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_modifier_modtext",
                table: "Modifiers",
                column: "ModifierText",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Modifiers");

            migrationBuilder.DropIndex(
                name: "idx_itemtype_category",
                table: "ItemTypes");

            migrationBuilder.DropIndex(
                name: "idx_itemtype_subcategory",
                table: "ItemTypes");
        }
    }
}
