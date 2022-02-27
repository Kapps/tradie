using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Tradie.Analyzer;

namespace Tradie.ItemLog.Tests {
	public static class LogRecordUtils {
		public static LogRecord CreateRecord(string stashTabId) {
			var offset = new ItemLogOffset(Guid.NewGuid().ToString());
			var tab = new AnalyzedStashTab(
				stashTabId,
				$"Rand{Guid.NewGuid():N}",
				$"Name{Guid.NewGuid():N}",
				$"Account{Guid.NewGuid():N}",
				"Scourge",
				"Premium",
				new ItemAnalysis[] {
					new(
						$"Item{Guid.NewGuid():N}",
						new ConcurrentDictionary<ushort, IAnalyzedProperties>(new Dictionary<ushort, IAnalyzedProperties>() {
							{ 3, new AnalyzedProps(TestUtils.TestUtils.RandomId<int>()) }
						})
					)
				}
			);

			return new LogRecord(offset, tab);
		}
		
		public record struct AnalyzedProps(int Foo) : IAnalyzedProperties;
	}
}