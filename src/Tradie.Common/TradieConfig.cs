using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tradie.Common {
	/// <summary>
	/// Basic config details that typically do not change frequently.
	/// </summary>
	public static class TradieConfig {
		/// <summary>
		/// Initializes the config either from SSM or from defaults, depending on the environment set.
		/// </summary>
		public static async Task InitializeFromEnvironment(IAmazonSimpleSystemsManagement ssmClient)  {
			string environment = System.Environment.GetEnvironmentVariable("TRADIE_ENV")
			                     ?? throw new ArgumentNullException("TRADIE_ENV");
			
			if(environment == "test") {
				InitializeWithDefaults(environment);
				DbHost = System.Environment.GetEnvironmentVariable("TRADIE_DB_HOST") ?? "127.0.0.1";
				DbUser = System.Environment.GetEnvironmentVariable("TRADIE_DB_USER") ?? "tradieadmin";
				DbPass = System.Environment.GetEnvironmentVariable("TRADIE_DB_PASS") ?? "tradie";
			} else {
				await InitializeFromSsm(environment, ssmClient);
			}
		}
		/// <summary>
		/// Initializes the TradieConfig with only default values assigned.
		/// No remote values will be loaded, however [DefaultValue] properties shall be assigned the default.
		/// </summary>
		public static void InitializeWithDefaults(string environment) {
			Console.WriteLine($"Initializing config with defaults for environment {environment}.");
			Environment = environment;
			var props = typeof(TradieConfig).GetProperties(BindingFlags.Public | BindingFlags.Static);
			foreach (var prop in props) {
				var defaultAttr = prop.GetCustomAttribute<DefaultValueAttribute>();
				if(defaultAttr == null)
					continue;
				
				prop.SetValue(null, defaultAttr.Value, null);
			}
		}
		/// <summary>
		/// Returns a TradieConfig with all properties loaded from SSM.
		/// If a property is not marked [DefaultValue], an error will be thrown if it is not present in SSM.
		/// </summary>
		public static async Task InitializeFromSsm(string environment, IAmazonSimpleSystemsManagement ssmClient) {
			Console.WriteLine($"Initializing config from SSM for environment {environment}.");
			var matchedProperties = new HashSet<PropertyInfo>();

			var props = typeof(TradieConfig).GetProperties(BindingFlags.Public | BindingFlags.Static)
				.Where(c => c.GetCustomAttribute<IgnoreDataMemberAttribute>() == null)
				.ToArray();
			var chunks = props.Chunk(10);
			
			// TODO: Should actually be the opposite where we load all params and then set the right ones.

			foreach(var chunk in chunks) {
				var req = new GetParametersRequest() {
					Names = chunk.Select(c => $"tradie-{environment}-Config.{c.Name}").ToList(),
					WithDecryption = true,
				};
				

				var resp = await ssmClient.GetParametersAsync(req);
				Console.WriteLine($"Got resp {SpanJson.JsonSerializer.Generic.Utf16.Serialize(resp)}");

				foreach(var param in resp.Parameters) {
					string propName = param.Name.Substring(param.Name.IndexOf(".") + 1);
					var prop = props.Single(c => c.Name == propName);
					object val = Convert.ChangeType(param.Value, prop.PropertyType);
					prop.SetValue(null, val);
					matchedProperties.Add(prop);
				}
			}

			foreach(var missingProp in props.Where(c=>!matchedProperties.Contains(c))) {
				var defaultAttr = missingProp.GetCustomAttribute<DefaultValueAttribute>()
					?? throw new ArgumentNullException(missingProp.Name);
				
				object? convertedDefault = Convert.ChangeType(defaultAttr.Value, missingProp.PropertyType);
				missingProp.SetValue(null, convertedDefault);
			}
		}

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		/// <summary>
		/// The user agent to include for all PoE API calls.
		/// </summary>
		[DefaultValue("User-Agent: OAuth tradie/1.0.0 (contact: tradie@ogi.bio) StrictMode")]
		public static string? UserAgent { get; set; }

		/// <summary>
		/// Timeout of any HTTP calls, in seconds.
		/// </summary>
		[DefaultValue(30)]
		public static int HttpTimeout { get; set; }

		/// <summary>
		/// The S3 bucket to store raw changesets in.
		/// </summary>
		public static string? ChangeSetBucket { get; set; }

		/// <summary>
		/// Prefix, including trailing slash, for the folder to write raw changesets to.
		/// </summary>
		[DefaultValue("raw/")]
		public static string? RawChangeSetPrefix { get; set; }

		/// <summary>
		/// Whether to enable detailed EF Core error messages.
		/// </summary>
		[DefaultValue(true)]
		public static bool DetailedSqlErrors { get; set; }

		// TODO: Add support for nested SSM params.
		/// <summary>
		/// Host for the analysis database.
		/// </summary>
		public static string? DbHost { get; set; }
		/// <summary>
		/// Username for the analysis database.
		/// </summary>
		public static string? DbUser { get; set; }
		/// <summary>
		/// Password for the analysis database.
		/// </summary>
		public static string? DbPass { get; set; }
		/// <summary>
		/// Name of the Kinesis stream used for sending analyzed item data.
		/// </summary>
		public static string AnalyzedItemStreamName { get; set; }

		/// <summary>
		/// Returns the environment that we're running under, such as "test" or "tradie-prod-ca".
		/// </summary>
		[IgnoreDataMember]
		public static string Environment { get; private set; }

	}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
