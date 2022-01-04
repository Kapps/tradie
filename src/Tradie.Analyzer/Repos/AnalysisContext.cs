using Microsoft.Data.Sqlite;
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

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
		optionsBuilder.EnableDetailedErrors(TradieConfig.DetailedSqlErrors);
		//optionsBuilder.LogTo(Console.WriteLine);
		
		optionsBuilder.UseNpgsql(new NpgsqlConnectionStringBuilder() {
			Database = "tradie",
			Timezone = "UTC",
			Pooling = true,
			SslMode = SslMode.Prefer,
			ApplicationName = "Tradie.Analyzer",
			Host = TradieConfig.DbHost,
			Username = TradieConfig.DbUser,
			Password = TradieConfig.DbPass,
			TrustServerCertificate = true,
			Timeout = 120,
			CommandTimeout = 120,
			InternalCommandTimeout = 120,
			MaxAutoPrepare = 60,
			MaxPoolSize = 3,
		}.ToString());

		
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
		base.OnModelCreating(modelBuilder);
	}
}