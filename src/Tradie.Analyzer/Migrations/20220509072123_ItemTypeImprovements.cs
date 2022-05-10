using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradie.Analyzer.Migrations
{
    public partial class ItemTypeImprovements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_itemtype_subcategory",
                table: "ItemTypes");

            migrationBuilder.AddColumn<string>(
                name: "IconUrl",
                table: "ItemTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string[]>(
                name: "Subcategories",
                table: "ItemTypes",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
            
            migrationBuilder.Sql("UPDATE \"ItemTypes\" SET \"Subcategories\" = ARRAY[\"Subcategory\"]");
            
            migrationBuilder.DropColumn(
	            name: "Subcategory",
	            table: "ItemTypes");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTypes_Subcategories",
                table: "ItemTypes",
                column: "Subcategories")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Properties",
                table: "Items",
                column: "Properties")
                //.Annotation("Npgsql:CreatedConcurrently", true)
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ItemTypes_Subcategories",
                table: "ItemTypes");

            migrationBuilder.DropIndex(
                name: "IX_Items_Properties",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IconUrl",
                table: "ItemTypes");

            migrationBuilder.DropColumn(
                name: "Subcategories",
                table: "ItemTypes");

            migrationBuilder.AddColumn<string>(
                name: "Subcategory",
                table: "ItemTypes",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "idx_itemtype_subcategory",
                table: "ItemTypes",
                column: "Subcategory");
        }
    }
}