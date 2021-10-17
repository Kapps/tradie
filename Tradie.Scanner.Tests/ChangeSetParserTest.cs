using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tradie.Scanner.Tests {
	[TestClass]
	public class ChangeSetParserTest {
		const string sampleTabPath = "sample-stashtab.json";
		const string expectedNextChangeId = "1295845615-1301399396-1257065509-1404516195-1351110417";

		[TestMethod]
		public async Task TestReadHeader() {
			var parser = new ChangeSetParser();
			using(var inputStream = File.OpenRead(sampleTabPath)) {
				var sw = Stopwatch.StartNew();
				var header = await parser.ReadHeader(inputStream);
				Assert.AreEqual(expectedNextChangeId, header.NextChangeSetId);
				System.Console.WriteLine($"Reading header took {sw.ElapsedMilliseconds}ms.");
			}
		}

		[TestMethod]
		public async Task TestReadStashTabs() {
			var parser = new ChangeSetParser();

			using var ms = new MemoryStream();
			using(var inputStream = File.OpenRead(sampleTabPath)) {
				var sw = Stopwatch.StartNew();

				await parser.ReadChanges(inputStream, ms);

				System.Console.WriteLine($"Reading all changes took {sw.ElapsedMilliseconds}ms.");

				using var doc = JsonDocument.Parse(Encoding.UTF8.GetString(ms.ToArray()));
			}
		}

		[TestMethod]
		public async Task TestReadFull() {
			var parser = new ChangeSetParser();

			using var ms = new MemoryStream();
			using(var inputStream = File.OpenRead(sampleTabPath)) {
				var sw = Stopwatch.StartNew();

				var header = await parser.ReadHeader(inputStream);
				Assert.AreEqual(expectedNextChangeId, header.NextChangeSetId);
				System.Console.WriteLine($"Reading header took {sw.ElapsedMilliseconds}ms.");

				await parser.ReadChanges(inputStream, ms);

				System.Console.WriteLine($"Reading all changes took {sw.ElapsedMilliseconds}ms.");

				using var doc = JsonDocument.Parse(Encoding.UTF8.GetString(ms.ToArray()));
			}
		}

		/*[TestMethod]
		public async Task TestReadStashTabs_Small() {
			var parser = new ChangeSetParser();
			string sampleData = 

			using var outStream = new MemoryStream();
			using(var inputStream = File.OpenRead(sampleTabPath)) {
				var sw = Stopwatch.StartNew();

				await parser.ReadChanges(inputStream, ms);

				System.Console.WriteLine($"Reading header took {sw.ElapsedMilliseconds}ms.");

				using var doc = JsonDocument.Parse(Encoding.UTF8.GetString(ms.ToArray()));
			}
		}*/
	}
}