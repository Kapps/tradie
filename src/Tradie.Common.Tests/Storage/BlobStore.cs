/*using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tradie.Common.Storage;

namespace Tradie.Common.Tests.Storage;

[TestClass]
public class BlobStoreTests {
	[DataRow(@"{""blobKey"": ""foo.gz""}", true, "foo.gz")]
	[DataRow(@"{""blobKey"": ""prefix/something_1/foo.json""}", true, "prefix/something_1/foo.json")]
	[DataRow(@"sometext\n{""blobKey"": ""prefix/something_1/foo.json""}", false, null)]
	[DataRow(@"foobar", false, null)]
	[DataRow(@"This is some random text.", false, null)]
	[DataTestMethod]
	public void TestKeyParsing(string input, bool valid, string key) {
		bool actualValid = BlobStore.TryGetRecordKey(input, out var actualKey);
		Assert.AreEqual(valid, actualValid);
		Assert.AreEqual(key, actualKey);
	}
}*/