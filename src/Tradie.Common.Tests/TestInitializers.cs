using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tradie.Common.Tests;

[TestClass]
public static class TestInitializer {
	[AssemblyInitialize]
	public static void InitializeAssembly(TestContext context) {
		Environment.SetEnvironmentVariable("TRADIE_ENV", "test");
		TradieConfig.InitializeFromEnvironment(null);
	}
}