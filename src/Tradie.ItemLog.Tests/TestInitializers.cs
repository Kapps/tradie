using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Tradie.Common;

namespace Tradie.ItemLog.Tests; 

/// <summary>
/// Initializer to set up environment and test config data.
/// </summary>
[TestClass]
public class TestInitializers {
	[AssemblyInitialize]
	public static async Task InitializeAssemblySetup(TestContext context) {
		Environment.SetEnvironmentVariable("TRADIE_ENV", "test");
		await TradieConfig.InitializeFromEnvironment(null!);
	}
}	