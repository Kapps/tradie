using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Tradie.Indexer.Tests;

[TestClass]
public static class TestInitializer {
	[AssemblyInitialize]
	public static void InitializeAssembly(TestContext context)
		=> TestUtils.TestInitializers.InitializeAssemblySetup(context);
}