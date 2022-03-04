using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using DeepEqual.Syntax;

namespace Tradie.Common.Tests {
	[TestClass]
	public class ParameterStoreTest {

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestInitialize]
		public void TestInitialize() {
			this._ssmClient = new(MockBehavior.Strict);
			this._store = new(this._ssmClient.Object);
		}

		[DataRow("Foo", "some string", "some string")]
		[DataRow("Number", "12", 12)]
		[DataRow("Long", "24", 24L)]
		[DataRow("Bool", "true", true)]
		[DataTestMethod]
		public async Task TestGetParameter(string name, string stringVal, object result) {
			var req = new GetParameterRequest() {
				Name = $"tradie-test.Param.{name}",
				WithDecryption = true,
			};
			var resp = new GetParameterResponse() {
				Parameter = new Parameter() {
					Value = stringVal,
				},
			};

			_ssmClient.Setup(c => c.GetParameterAsync(It.Is<GetParameterRequest>(m => m.IsDeepEqual(req)), default))
				.ReturnsAsync(resp);

			var method = _store.GetType().GetMethod("GetParameter").MakeGenericMethod(result.GetType());
			var task = (Task)method.Invoke(this._store, new object[] { name, null });
			await task;

			var param = task.GetType().GetProperty("Result").GetValue(task);

			var value = param.GetType().GetField("Value").GetValue(param);

			Assert.AreEqual(result, value);

			_ssmClient.VerifyAll();
		}

		private SsmParameterStore _store;
		private Mock<IAmazonSimpleSystemsManagement> _ssmClient;
	}
}