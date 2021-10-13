using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Tradie.Common {
	/// <summary>
	/// Basic config details that typically do not change frequently.
	/// </summary>
	public class TradieConfig {
		/// <summary>
		/// Returns a TradieConfig with all properties loaded from SSM.
		/// If a property is not marked [DefaultValue], an error will be thrown if it is not present in SSM.
		/// </summary>
		public static async Task<TradieConfig> LoadFromSSM(IAmazonSimpleSystemsManagement ssmClient) {
			var res = new TradieConfig();
			HashSet<PropertyInfo> matchedProperties = new HashSet<PropertyInfo>();

			var props = res.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var chunks = props.Chunk(10);

			foreach(var chunk in chunks) {
				var req = new GetParametersRequest() {
					Names = chunk.Select(c => c.Name).ToList(),
					WithDecryption = true,
				};

				var resp = await ssmClient.GetParametersAsync(req);

				foreach(var param in resp.Parameters) {
					string propName = param.Name.Substring(param.Name.IndexOf(".") + 1);
					var prop = props.Single(c => c.Name == propName);
					object val = Convert.ChangeType(param.Value, prop.PropertyType);
					prop.SetValue(res, val);
					matchedProperties.Add(prop);
				}
			}

			foreach(var missingProp in props.Where(c=>!matchedProperties.Contains(c))) {
				var defaultAttr = missingProp.GetCustomAttribute<DefaultValueAttribute>()
					?? throw new ArgumentNullException(missingProp.Name);
				
				object? convertedDefault = Convert.ChangeType(defaultAttr.Value, missingProp.PropertyType);
				missingProp.SetValue(res, convertedDefault);
			}

			return res;
			
		}

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		/// <summary>
		/// The user agent to include for all PoE API calls.
		/// </summary>
		[DefaultValue("User-Agent: OAuth tradie/1.0.0 (contact: tradie@ogi.bio) StrictMode")]
		public string UserAgent { get; }

		/// <summary>
		/// Timeout of any HTTP calls, in seconds.
		/// </summary>
		[DefaultValue(30)]
		public int HttpTimeout { get; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	}
}
