using HashiCorp.Cdktf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tradie.Infrastructure;

namespace Tradie.Infrastructure.Tests {
	[TestClass]
	public class InfrastructureTests {
		[TestMethod]
		public void TestSynthesize() {
			var app = Testing.App();
			var stack = new MyApp(app, "Tradie.Infrastructure");
			var res = Testing.FullSynth(stack);
			Console.WriteLine(res);
		}
	}
}