using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradie.Common {
	/// <summary>
	/// Allows retrieving and storing dynamic parameters.
	/// </summary>
	public interface IParameterStore {
		/// <summary>
		/// GetParameter returns the parameter with the given key, or default if not found.
		/// If the key was stored as a different value, it is attempted to be converted to TValue.
		/// </summary>
		public Task<DynamicParameter<TValue?>> GetParameter<TValue>(string key, TValue? @default = default);
		/// <summary>
		/// SetParameter inserts or updates the value of a parameter to the given value.
		/// Multiple requests may result in concurrent updates and overwriting a different value than the one retrieved.
		/// </summary>
		public Task SetParameter<T>(string key, T value);
	}

	public class SsmParameterStore : IParameterStore {
		public SsmParameterStore(IAmazonSimpleSystemsManagement ssmClient) {
			this.ssmClient = ssmClient;
		}

		public async Task<DynamicParameter<TValue?>> GetParameter<TValue>(string key, TValue? defaultValue = default) {
			var req = new GetParameterRequest() {
				Name = key,
				WithDecryption = true,
			};

			try {
				var resp = await ssmClient.GetParameterAsync(req);
				string valString = resp.Parameter.Value;
				TValue converted = (TValue)Convert.ChangeType(valString, typeof(TValue));
				return new(key, converted);
			} catch(ParameterNotFoundException) {
				return new(key, defaultValue);
			}
		}

		public async Task SetParameter<TValue>(string key, TValue value) {
			string? valString = Convert.ToString(value);
			var req = new PutParameterRequest() {
				Type = "String",
				Name = key,
				Tier = ParameterTier.Standard,
				Value = valString,
				Overwrite = true,
			};

			var resp = await ssmClient.PutParameterAsync(req);
			if(resp.HttpStatusCode != System.Net.HttpStatusCode.OK) {
				throw new ParameterExecutionException();
			}
		}

		private IAmazonSimpleSystemsManagement ssmClient;
	}

	[System.Serializable]
	public class ParameterExecutionException : Exception {
		public ParameterExecutionException() : base("Failed to make request for parameter operation.") { }
		public ParameterExecutionException(string message) : base(message) { }
		public ParameterExecutionException(string message, Exception inner) : base(message, inner) { }
		protected ParameterExecutionException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
