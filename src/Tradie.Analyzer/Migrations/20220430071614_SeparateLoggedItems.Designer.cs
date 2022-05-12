﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Tradie.Analyzer;
using Tradie.Analyzer.Repos;

#nullable disable

namespace Tradie.Analyzer.Migrations
{
    [DbContext(typeof(AnalysisContext))]
    [Migration("20220430071614_SeparateLoggedItems")]
    partial class SeparateLoggedItems
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Tradie.Analyzer.Entities.ItemType", b =>
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

                    b.HasIndex(new[] { "Category" }, "idx_itemtype_category");

                    b.HasIndex(new[] { "Name" }, "idx_itemtype_name")
                        .IsUnique();

                    b.HasIndex(new[] { "Subcategory" }, "idx_itemtype_subcategory");

                    b.ToTable("ItemTypes");
                });

            modelBuilder.Entity("Tradie.Analyzer.Entities.LoggedItem", b =>
                {
                    b.Property<long>("IdHash")
                        .HasColumnType("bigint");

                    b.Property<Dictionary<ushort, IAnalyzedProperties>>("Properties")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("RawId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RawStashTabId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("IdHash");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("Tradie.Analyzer.Entities.LoggedStashTab", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Kind")
                        .HasColumnType("text");

                    b.Property<string>("LastCharacterName")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("League")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Owner")
                        .HasColumnType("text");

                    b.Property<byte[]>("PackedItems")
                        .HasColumnType("bytea");

                    b.Property<string>("RawId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Created" }, "idx_stash_Created");

                    b.HasIndex(new[] { "LastModified" }, "idx_stash_LastModified");

                    b.HasIndex(new[] { "League" }, "idx_stash_League");

                    b.HasIndex(new[] { "RawId" }, "idx_stash_RawId")
                        .IsUnique();

                    b.ToTable("StashTabs");
                });

            modelBuilder.Entity("Tradie.Analyzer.Entities.Modifier", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("ModHash")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("ModifierText")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "ModHash" }, "idx_modifier_modhash")
                        .IsUnique();

                    b.HasIndex(new[] { "ModifierText" }, "idx_modifier_modtext")
                        .IsUnique();

                    b.ToTable("Modifiers");
                });

            modelBuilder.Entity("Tradie.Analyzer.Entities.ItemType", b =>
                {
                    b.OwnsOne("Tradie.Analyzer.Entities.Requirements", "Requirements", b1 =>
                        {
                            b1.Property<int>("ItemTypeId")
                                .HasColumnType("integer");

                            b1.Property<int>("Dex")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer")
                                .HasDefaultValue(0)
                                .HasColumnName("DexRequirement");

                            b1.Property<int>("Int")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer")
                                .HasDefaultValue(0)
                                .HasColumnName("IntRequirement");

                            b1.Property<int>("Level")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer")
                                .HasDefaultValue(0)
                                .HasColumnName("LevelRequirement");

                            b1.Property<int>("Str")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer")
                                .HasDefaultValue(0)
                                .HasColumnName("StrRequirement");

                            b1.HasKey("ItemTypeId");

                            b1.ToTable("ItemTypes");

                            b1.WithOwner()
                                .HasForeignKey("ItemTypeId");
                        });

                    b.Navigation("Requirements")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}