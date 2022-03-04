using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Tradie.Analyzer.Tests {
	[TestClass]
	public static class TestInitializer {
		[AssemblyInitialize] public static void InitializeAssembly(TestContext context)
			=> TestUtils.TestInitializers.InitializeAssemblySetup(context);
	}
}