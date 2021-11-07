using RateLimiter;
using RestSharp;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Tradie.Common {
	/// <summary>
	/// Client used for making calls to the Path of Exile API.
	/// </summary>
	public interface IApiClient {
		/// <summary>
		/// Makes a GET request to the endpoint with the given form content, returning a stream containing the body of the ersponse.
		/// </summary>
		Task<string> Get(string endpoint, params (string key, string val)[] form);
	}

	public class ApiClient : IApiClient {
		public ApiClient(TradieConfig config) {
			_config = config;
			_httpClient = new HttpClient() {
				BaseAddress = new Uri("https://www.pathofexile.com/api/"),
				Timeout = TimeSpan.FromSeconds(config.HttpTimeout),
			};
			_httpClient.DefaultRequestHeaders.Add("User-Agent", "tradie/0.1.0 (contact: tradie@ogi.bio) StrictMode");
			_timeLimiter = TimeLimiter.GetFromMaxCountByInterval(2, TimeSpan.FromSeconds(1));
		}

		public async Task<string> Get(string endpoint, params (string key, string val)[] form) {
			var urlBuilder = new StringBuilder(endpoint);
			if(form.Length > 0) {
				urlBuilder.Append("?");
				foreach(var kvp in form) {
					urlBuilder.Append(UrlEncoder.Default.Encode(kvp.key));
					urlBuilder.Append("=");
					urlBuilder.Append(UrlEncoder.Default.Encode(kvp.val));
				}
			}

			var req = new HttpRequestMessage(HttpMethod.Get, urlBuilder.ToString()) {
				Content = new FormUrlEncodedContent(form.Select((pair) => new KeyValuePair<string, string>(pair.key, pair.val))),
			};

			return await _timeLimiter.Enqueue(async () => {
				var resp = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
				if(!resp.IsSuccessStatusCode) {
					throw new HttpRequestException($"Endpoint {endpoint} failed with status code {resp.StatusCode}.");
				}

				return await resp.Content.ReadAsStringAsync();
			});
		}

		private TradieConfig _config;
		private HttpClient _httpClient;
		private TimeLimiter _timeLimiter;
	}
}
