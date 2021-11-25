using DeepEqual.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using Tradie.Common.RawModels;

namespace Tradie.Analyzer.Tests.Analyzers; 

/// <summary>
/// Helper functions for various tests.
/// </summary>
public static class TestUtils {
	/// <summary>
	/// Reads an item from the Analyzer json folder with the given file name (excluding extension).
	/// </summary>
	public static async Task<Item> ReadTestItem(string itemName) {
		string contents = await File.ReadAllTextAsync($"SampleItems/{itemName}.json");
		return SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Item>(contents);
	}

	/// <summary>
	/// Creates a logger of type T that logs to the output window.
	/// </summary>
	public static ILogger<T> CreateLogger<T>() {
		var serviceProvider = new ServiceCollection()
			.AddLogging(builder=>builder.AddDebug())
			.BuildServiceProvider();

		var factory = serviceProvider.GetService<ILoggerFactory>()!;

		var logger = factory.CreateLogger<T>()!;
		return logger;
	}

	/// <summary>
	/// Returns a matcher that indicates whether a value deep equals the expected value. 
	/// </summary>
	public static T DeepEquals<T>(T expected) {
		bool Validate(T actual) {
			return actual.WithDeepEqual(expected)
				.IgnoreProperty(reader => reader.Name == "Id")
				.Compare();
		}

		return Match.Create<T>(Validate);
	}
}