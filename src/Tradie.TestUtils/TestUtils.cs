using DeepEqual.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace Tradie.TestUtils;

/// <summary>
/// Helper functions for various tests.
/// </summary>
public static class TestUtils {
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
	public static T DeepMatcher<T>(this T expected, params string[] ignoredProperties) {
		bool Validate(T actual) {
			return actual.WithDeepEqual(expected)
				.IgnoreProperty(reader => reader.Name == "Id" || ignoredProperties.Contains(reader.Name))
				.WithCustomComparison(new DateClosenessComparer())
				.WithCustomComparison(new StreamComparer())
				.Compare();
		}

		return Match.Create<T>(Validate);
	}

	/// <summary>
	/// Returns a random unique ID that has not been used of the given type.
	/// </summary>
	public static T RandomId<T>() where T : struct, IComparable<T> {
		T min = (T)((dynamic)default(T) + 1);
		T max = (T)Convert.ChangeType(
			typeof(T).GetField("MaxValue", BindingFlags.Static | BindingFlags.Public)!.GetValue(null)!,
			typeof(T)
		)!;

		T res;
		do {
			res = (T)(dynamic)(((ulong)Rng.NextInt64() % (Convert.ToUInt64(max) -1)) + 1);
		} while(!UsedIds.TryAdd(Convert.ToUInt64(res), true));

		return res;
	}

	/// <summary>
	/// Returns a CancellationToken that expires in 30 seconds.
	/// </summary>
	public static CancellationToken CreateCancellationToken() {
		var src = new CancellationTokenSource(TimeSpan.FromSeconds(30));
		return src.Token;
	}

	private static readonly ConcurrentDictionary<ulong, bool> UsedIds = new();
	private static readonly Random Rng = new Random();
}
