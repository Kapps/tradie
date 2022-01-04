using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Reflection;

namespace Tradie.TestUtils {
	/// <summary>
	/// <para>A base class to implement for test classes to reduce boilerplate.</para>
	/// <para>Currently, this class handles the following:</para>
	/// <list type="bullet">
	///	<item>
	///		<description>Instantiates all mocks and recreates them for each test, with strict mode.</description>
	///	</item>
	/// <item>
	///		<description>Calls VerifyAll on all mocks after each test.</description>
	/// </item>
	/// </list>
	/// </summary>
	[TestClass]
	public abstract class TestBase {
		[TestInitialize]
		public void BaseInitializer() {
			this.InstantiateMocks();
		}
		
		[TestCleanup]
		public void BaseCleaner() {
			this.AssertMocks();
		}

		protected virtual void AssertMocks() {
			foreach(var mockField in MockFields) {
				dynamic instance = mockField.GetValue(this)!;
				instance.VerifyAll();
			}
		}
		
		protected virtual void InstantiateMocks() {
			foreach(var mockField in MockFields) {
				var ctor = mockField.FieldType.GetConstructor(new[] {typeof(MockBehavior)});
				var instance = ctor.Invoke(new object[] {MockBehavior.Strict});
				mockField.SetValue(this, instance);
			}
		}

		private IEnumerable<FieldInfo> MockFields => this.GetType()
				.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance)
				.Where(c => c.FieldType.IsGenericType && c.FieldType.GetGenericTypeDefinition() == typeof(Mock<>))
				.ToArray();
	}
}