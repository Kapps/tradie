﻿using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Models;
using Tradie.Common;

namespace Tradie.Analyzer.Repos;

/// <summary>
/// DataContext for entities resulting from analysis of raw items.
/// </summary>
public class AnalysisContext : DbContext {
	/// <summary>
	/// Returns any base item types that have so far been analyzed.
	/// </summary>
	public DbSet<ItemType> ItemTypes { get; set; } = null!;

	/// <summary>
	/// Returns any affix modifiers that have so far been analyzed.
	/// </summary>
	public DbSet<Modifier> Modifiers { get; set; } = null!;

	/// <summary>
	/// Returns all stash tabs logged to the context.
	/// </summary>
	public DbSet<LoggedStashTab> LoggedStashTabs { get; set; } = null!;

	/// <summary>
	/// Returns all items contained within the logged stash tabs.
	/// </summary>
	public DbSet<LoggedItem> LoggedItems { get; set; } = null!;

	/// <summary>
	/// Returns the set of affix ranges in the context.
	/// </summary>
	public DbSet<AffixRange> AffixRanges { get; set; } = null!;

	/// <summary>
	/// Returns the set of item price histories in the context.
	/// </summary>
	public DbSet<ItemPriceHistory> ItemPriceHistories { get; set; } = null!;

	/// <summary>
	/// Returns a connection string that can be used to connect to the analysis database.
	/// </summary>
	public static NpgsqlConnectionStringBuilder CreateConnectionStringBuilder() {
		return new NpgsqlConnectionStringBuilder() {
			Database = Environment.GetEnvironmentVariable("TRADIE_DB_NAME") ?? TradieConfig.DbName,
			Timezone = "UTC",
			Pooling = true,
			SslMode = SslMode.Prefer,
			ApplicationName = "Tradie.Analyzer",
			Host = Environment.GetEnvironmentVariable("TRADIE_DB_HOST") ?? TradieConfig.DbHost,
			Username = Environment.GetEnvironmentVariable("TRADIE_DB_USER") ?? TradieConfig.DbUser,
			Password = Environment.GetEnvironmentVariable("TRADIE_DB_PASSWORD") ?? TradieConfig.DbPass,
			TrustServerCertificate = true,
			Timeout = 20,
			CommandTimeout = 120,
			InternalCommandTimeout = 120,
			MaxAutoPrepare = 60,
			MaxPoolSize = 10,
			IncludeErrorDetail = true,
			Enlist = true,
			TcpKeepAlive = true,
			TcpKeepAliveTime = 10,
			KeepAlive = 10,
			ConnectionIdleLifetime = 30,
			//Multiplexing = true
		};
	}

	public AnalysisContext() { }

	public AnalysisContext(DbContextOptions options) : base(options) { }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
		optionsBuilder.EnableDetailedErrors(TradieConfig.DetailedSqlErrors);
#if DEBUG
		//optionsBuilder.LogTo(Console.WriteLine);
#endif

		optionsBuilder.UseNpgsql(CreateConnectionStringBuilder().ToString());
		optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);

		base.OnConfiguring(optionsBuilder);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<ItemType>()
			.Property(c => c.Id).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Throw);
		modelBuilder.Entity<ItemType>()
			.OwnsOne(c => c.Requirements, b => {
				b.Property(c => c.Dex).HasColumnName("DexRequirement").HasDefaultValue(0).IsRequired();
				b.Property(c => c.Int).HasColumnName("IntRequirement").HasDefaultValue(0).IsRequired();
				b.Property(c => c.Str).HasColumnName("StrRequirement").HasDefaultValue(0).IsRequired();
				b.Property(c => c.Level).HasColumnName("LevelRequirement").HasDefaultValue(0).IsRequired();
			});
		modelBuilder.Entity<LoggedItem>()
			.HasIndex(c => c.Properties)
			.HasMethod("GIN")
			.IsCreatedConcurrently();
		modelBuilder.Entity<ItemType>()
			.HasIndex(c => c.Subcategories)
			.HasMethod("GIN");
		modelBuilder.Entity<AffixRange>()
			.HasKey(c => new { c.ModHash, c.ModCategory, c.EntityKind });
		modelBuilder.Entity<Modifier>()
			.HasData(PseudoMods.AllPseudoModifiers);
		modelBuilder.Entity<ItemPriceHistory>()
			.HasIndex(nameof(ItemPriceHistory.ItemId), nameof(ItemPriceHistory.RecordedTime))
			.HasSortOrder(SortOrder.Ascending, SortOrder.Descending);
		modelBuilder.Entity<ItemPriceHistory>()
			.HasKey(nameof(ItemPriceHistory.ItemId), nameof(ItemPriceHistory.RecordedTime));
		modelBuilder.Entity<ItemPriceHistory>()
			.Property(c => c.RecordedTime);

		base.OnModelCreating(modelBuilder);
	}
}