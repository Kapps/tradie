using RestSharp;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tradie.Common {
	/// <summary>
	/// Client used for making calls to the Path of Exile API.
	/// </summary>
	public interface IApiClient {
		/// <summary>
		/// Makes a GET request to the endpoint with the given form content, returning a stream containing the body of the ersponse.
		/// </summary>
		Task<Stream> GetStream(string endpoint, params (string key, string val)[] form);
	}

	public class ApiClient : IApiClient {
		public ApiClient(TradieConfig config) {
			_config = config;
			_httpClient = new HttpClient() {
				BaseAddress = new Uri("https://www.pathofexile.com/api"),
				Timeout = TimeSpan.FromSeconds(config.HttpTimeout),
			};
		}

		public async Task<Stream> GetStream(string endpoint, params (string key, string val)[] form) {
			var req = new HttpRequestMessage(HttpMethod.Get, endpoint);
			req.Content = new FormUrlEncodedContent(form.Select((pair) => new System.Collections.Generic.KeyValuePair<string, string>(pair.key, pair.val)));

			var resp = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);

			return await resp.Content.ReadAsStreamAsync();
		}

		private TradieConfig _config;
		private HttpClient _httpClient;
	}
}
