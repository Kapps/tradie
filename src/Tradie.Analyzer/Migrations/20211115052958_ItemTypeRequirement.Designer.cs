﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Tradie.Analyzer.Repos;

#nullable disable

namespace Tradie.Analyzer.Migrations
{
    [DbContext(typeof(AnalysisContext))]
    [Migration("20211115052958_ItemTypeRequirement")]
    partial class ItemTypeRequirement
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Tradie.Analyzer.Models.ItemType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Category")
                        .HasColumnType("text");

                    b.Property<int>("Height")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Subcategory")
                        .HasColumnType("text");

                    b.Property<int>("Width")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Name" }, "idx_itemtype_name")
                        .IsUnique();

                    b.ToTable("ItemTypes");
                });

            modelBuilder.Entity("Tradie.Analyzer.Models.ItemType", b =>
                {
                    b.OwnsOne("Tradie.Analyzer.Models.Requirements", "Requirements", b1 =>
                        {
                            b1.Property<int>("ItemTypeId")
                                .HasColumnType("integer");

                            b1.HasKey("ItemTypeId");

                            b1.ToTable("ItemTypes");

                            b1.WithOwner()
                                .HasForeignKey("ItemTypeId");
                        });

                    b.Navigation("Requirements");
                });
#pragma warning restore 612, 618
        }
    }
}
