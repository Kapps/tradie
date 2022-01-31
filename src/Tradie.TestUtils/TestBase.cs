using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Reflection;
using System.Transactions;
using Tradie.Analyzer.Repos;

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
		// NOTE: Initializer and Cleanup must be void, and _can not_ be async Tasks otherwise they are invoked
		// on the wrong thread, and TransactionScopes end up applying in peculiar ways.
		
		[TestInitialize]
		public void BaseInitializer() {
			this.InstantiateContexts();
			this.InstantiateMocks();
			this.Initialize();
		}
		
		[TestCleanup]
		public void BaseCleaner() {
			this.Cleanup();
			this.AssertMocks();
			this.DisposeContexts();
		}

		/// <summary>
		/// A method to override to implement additional initialization after mocks and other resources are.
		/// </summary>
		protected virtual void Initialize() { }
		
		/// <summary>
		/// A method to override to implement additional cleanup before mocks and other resources are.
		/// </summary>
		protected virtual void Cleanup() { }

		protected virtual void DisposeContexts() {
			foreach(var contextField in ContextFields) {
				var instance = (DbContext)contextField.GetValue(this)!;
				instance.Dispose();
			}
			
			Console.WriteLine("Exiting");
			Transaction.Current!.Rollback();
			this.TransactionScope.Dispose();
		}

		protected virtual void AssertMocks() {
			foreach(var mockField in MockFields) {
				dynamic instance = mockField.GetValue(this)!;
				instance.VerifyAll();
			}
		}

		protected virtual void InstantiateContexts() {
			this.TransactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
			Console.WriteLine("Entered");
			foreach(var contextField in ContextFields) {
				var ctor = contextField.FieldType.GetConstructor(new Type[] { })!;
				var instance = (DbContext)ctor.Invoke(new object[] { });
				contextField.SetValue(this, instance);
				instance.Database.AutoTransactionsEnabled = false;
			}
		}
		
		protected virtual void InstantiateMocks() {
			foreach(var mockField in MockFields) {
				var ctor = mockField.FieldType.GetConstructor(new[] {typeof(MockBehavior)})!;
				var instance = ctor.Invoke(new object[] {MockBehavior.Strict});
				mockField.SetValue(this, instance);
			}
		}

		private IEnumerable<FieldInfo> MockFields => this.GetType()
			.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy |
			           BindingFlags.Instance)
			.Where(c => c.FieldType.IsGenericType && c.FieldType.GetGenericTypeDefinition() == typeof(Mock<>))
			.ToArray();

		private IEnumerable<FieldInfo> ContextFields => this.GetType()
			.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy |
			           BindingFlags.Instance)
			.Where(c => c.FieldType == typeof(AnalysisContext))
			.ToArray();

		protected TransactionScope TransactionScope = null!;
	}
}