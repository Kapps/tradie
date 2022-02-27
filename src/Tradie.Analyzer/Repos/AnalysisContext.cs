﻿using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;
using Tradie.Analyzer.Entities;
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

	/*/// <summary>
	/// Returns all analyzed items logged to the context.
	/// </summary>
	public DbSet<LoggedItem> LoggedItems { get; set; } = null!;*/

	/// <summary>
	/// Returns all stash tabs logged to the context.
	/// </summary>
	public DbSet<LoggedStashTab> LoggedStashTabs { get; set; } = null!;

	/// <summary>
	/// Returns a connection string that can be used to connect to the analysis database.
	/// </summary>
	public static string CreateConnectionString() {
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
			Timeout = 120,
			CommandTimeout = 120,
			InternalCommandTimeout = 120,
			MaxAutoPrepare = 60,
			MaxPoolSize = 5,
			IncludeErrorDetail = true,
			Enlist = true,
			TcpKeepAlive = true,
			TcpKeepAliveTime = 10,
			KeepAlive = 10,
			ConnectionIdleLifetime = 30
		}.ToString();
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
		optionsBuilder.EnableDetailedErrors(TradieConfig.DetailedSqlErrors);
		//optionsBuilder.LogTo(Console.WriteLine);
		
		optionsBuilder.UseNpgsql(CreateConnectionString());

		
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
		/*modelBuilder.Entity<LoggedItem>()
			.HasIndex(c => c.Properties, "idx_item_Properties")
			.HasMethod("GIN");*/
		modelBuilder.Entity<LoggedStashTab>()
			.HasIndex(c => c.Items, "idx_tab_Items")
			.HasMethod("GIN");
		base.OnModelCreating(modelBuilder);
	}
}