using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Tradie.Common.Tests;

[TestClass]
public class InitializersTests {
	[TestMethod]
	public async Task TestInitializeOnce_Serial() {
		int count = 0; 
		async Task Initializer() {
			await Task.Delay(10);
			count++;
		};

		Assert.AreEqual(0, count);
		await Initializers.InitializeOnce(Initializer);
		Assert.AreEqual(1, count);
		await Initializers.InitializeOnce(Initializer);
		Assert.AreEqual(1, count);
	}

	[TestMethod]
	public async Task TestInitializeOnce_Concurrent() {
		int count = 0; 
		async Task Initializer() {
			await Task.Delay(100);
			count++;
		};

		var tasks = Enumerable.Range(1, 100)
			.Select(c => Initializers.InitializeOnce(Initializer));

		await Task.WhenAll(tasks);
		
		Assert.AreEqual(1, count);
	}

	[TestCleanup]
	public void ResetInitializers() {
		typeof(Initializers)
			.GetField("_initialized", BindingFlags.Static | BindingFlags.NonPublic)
			.SetValue(false, null);
	}
}