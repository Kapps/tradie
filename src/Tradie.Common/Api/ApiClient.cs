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
		/// Makes a GET request to the endpoint with the given form content, returning a stream containing the body of the response.
		/// </summary>
		Task<string> Get(string endpoint, params (string key, string val)[] form);
	}
	
	public class ApiClient : IApiClient {
		public ApiClient(TimeLimiter timeLimiter, string baseUrl) {
			_httpClient = new HttpClient() {
				BaseAddress = new Uri(baseUrl),
				Timeout = TimeSpan.FromSeconds(TradieConfig.HttpTimeout),
			};
			_httpClient.DefaultRequestHeaders.Add("User-Agent", TradieConfig.UserAgent);
			_timeLimiter = timeLimiter;
		}

		public async Task<string> Get(string endpoint, params (string key, string val)[] form) {
			var urlBuilder = new StringBuilder(endpoint);
			if(form.Length > 0) {
				urlBuilder.Append("?");
				bool first = true;
				foreach(var kvp in form) {
					if(!first) {
						urlBuilder.Append("&");
					} else {
						first = false;
					}
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

		private readonly HttpClient _httpClient;
		private readonly TimeLimiter _timeLimiter;
	}
}
