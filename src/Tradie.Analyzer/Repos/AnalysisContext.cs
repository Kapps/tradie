using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;
using Tradie.Analyzer.Models;
using Tradie.Common;

namespace Tradie.Analyzer.Repos; 

/// <summary>
/// DataContext for entities resulting from analysis of raw items.
/// </summary>
public class AnalysisContext : DbContext {
	public DbSet<ItemType> ItemTypes { get; set; } = null!;

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
		optionsBuilder.EnableDetailedErrors(TradieConfig.DetailedSqlErrors);

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
		}.ToString());

		
		base.OnConfiguring(optionsBuilder);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<ItemType>()
			.Property(c => c.Id).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Throw);
		modelBuilder.Entity<ItemType>()
			.OwnsOne<Requirements>(c => c.Requirements);
		base.OnModelCreating(modelBuilder);
	}
}