using Amazon.SimpleSystemsManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
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
		Environment.SetEnvironmentVariable("TRADIE_ENV", "test");
		await TradieConfig.InitializeFromEnvironment(null!);

		await using var dbContext = new AnalysisContext();
		await dbContext.Database.MigrateAsync();
	}
}	