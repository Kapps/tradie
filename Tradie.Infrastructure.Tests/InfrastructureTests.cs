using HashiCorp.Cdktf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
using Tradie.Infrastructure;

namespace Tradie.Infrastructure.Tests {
	[TestClass]
	public class InfrastructureTests {
		[TestMethod]
		public void TestSynthesize() {
			var app = Testing.App();
			var stack = new MyApp(app, "Tradie.Infrastructure", new ResourceConfig() {
				Environment = "test",
				Region = "ca-central-1",
				Version = "0.1.0",
				BaseDirectory = Path.GetFullPath("../"),
				LocalIpAddress = IPAddress.Loopback,
			});
			var res = Testing.FullSynth(stack);
			Console.WriteLine(res);
		}
	}
}