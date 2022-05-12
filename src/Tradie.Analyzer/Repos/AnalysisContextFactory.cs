using Amazon.SimpleSystemsManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Tradie.Common;

namespace Tradie.Analyzer.Repos;

public class AnalysisContextFactory : IDesignTimeDbContextFactory<AnalysisContext> {
	public AnalysisContext CreateDbContext(string[] args) {
		var ssm = new AmazonSimpleSystemsManagementClient();
		TradieConfig.InitializeFromEnvironment(ssm).GetAwaiter().GetResult();

		var connStringBuilder = AnalysisContext.CreateConnectionStringBuilder();
		connStringBuilder.CommandTimeout = 0;
		
		var optionsBuilder = new DbContextOptionsBuilder();
		//optionsBuilder.EnableDetailedErrors(TradieConfig.DetailedSqlErrors);
		//optionsBuilder.LogTo(Console.WriteLine);
		
		optionsBuilder.UseNpgsql(connStringBuilder.ToString());
		optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

		return new AnalysisContext(optionsBuilder.Options);
	}
}