using Amazon.SimpleSystemsManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Tradie.Analyzer.Repos;
using Tradie.Common;

namespace Tradie.Analyzer.Tests; 

/// <summary>
/// Initializer to set up environment and test config data.
/// </summary>
[TestClass]
public class TestInitializers {
	[AssemblyInitialize]
	public static async Task InitializeAssemblySetup(TestContext context) {
		TradieConfig.InitializeWithDefaults("test");
		TradieConfig.DbHost = "localhost";
		TradieConfig.DbUser = "tradie";
		TradieConfig.DbPass = "tradie";

		using var dbContext = new AnalysisContext();
		await dbContext.Database.MigrateAsync();
	}
}