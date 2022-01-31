using Amazon.SimpleSystemsManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using Tradie.Analyzer.Repos;
using Tradie.Common;

namespace Tradie.TestUtils; 

/// <summary>
/// Initializer to set up environment and test config data.
/// Because assembly initializers are only called on the active assembly, this method must be called in an
/// AssemblyInitializer of all referencing projects.
/// </summary>
public static class TestInitializers {
	public static void InitializeAssemblySetup(TestContext context) {
		Environment.SetEnvironmentVariable("TRADIE_ENV", "test");
		TradieConfig.InitializeWithDefaults("test");

		using var dbContext = new AnalysisContext();
		Console.WriteLine(dbContext.Database.GetConnectionString());
		dbContext.Database.Migrate();
	}
}	