using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using Newtonsoft.Json.Linq;




namespace Movolira.DataProviders {
	public static class JSONHelper {
		private static HttpClient _http_client;
		private const int HTTP_RETRY_COUNT = 5;
		private const int HTTP_RETRY_DELAY = 500; // milliseconds




		static JSONHelper() {
			// These if clauses prevent the objects from being reinitialized when not needed.
			// (when the app is recovering from a saved instance state)
			if (_http_client == null) {
				_http_client = new HttpClient();
				// Default buffer size results in over 200MB additional taken storage space
				_http_client.MaxResponseContentBufferSize = 256000; // bytes
			}


			if (BlobCache.ApplicationName != "Movolira") {
				BlobCache.ApplicationName = "Movolira";
			}
		}




		public static async Task<JObject> getJson(string cache_id, Uri json_uri) {
			JObject json;
			try {
				json = await BlobCache.LocalMachine.GetObject<JObject>(cache_id);
			} catch (Exception) {
				json = new JObject();


				for (int i_retry = 0; i_retry < HTTP_RETRY_COUNT; ++i_retry) {
					HttpResponseMessage json_response;
					try {
						json_response = await _http_client.GetAsync(json_uri);
					} catch (Exception) {
						await Task.Delay(HTTP_RETRY_DELAY);
						continue;
					}


					if (!json_response.IsSuccessStatusCode) {
						await Task.Delay(HTTP_RETRY_DELAY);
						continue;
					}


					string json_data = json_response.Content.ReadAsStringAsync().Result;
					JObject json_object = JObject.Parse(json_data);
					json.Add("data", json_object);
					json_response.Dispose();
					await BlobCache.LocalMachine.InsertObject(cache_id, json,
						new DateTimeOffset(DateTime.Now.AddDays(7)));
					break;
				}


			}
			return json;
		}




		public static T getJTokenValue<T>(JToken jtoken, string key) {
			T value = default;
			if (doesJTokenContainKey(jtoken, key)) {
				value = jtoken[key].Value<T>();
			}
			return value;
		}




		public static List<T> getJTokenValueList<T>(JToken jtoken, string key) {
			var value_list = new List<T>();
			if (doesJTokenContainKey(jtoken, key)) {
				value_list = jtoken[key].Children().Values<T>().ToList();
			}
			return value_list;
		}




		public static List<JToken> getJTokenList(JToken jtoken, string key) {
			var jtoken_list = new List<JToken>();
			if (doesJTokenContainKey(jtoken, key)) {
				jtoken_list = jtoken[key].Children().ToList();
			}
			return jtoken_list;
		}




		public static bool doesJTokenContainKey(JToken jtoken, string key) {
			if (jtoken[key] != null) {
				if (jtoken[key].Type != JTokenType.Null) {
					return true;
				}
			}
			return false;
		}




		public static bool doesJsonContainData(JObject json) {
			if (json == null) {
				return false;
			}
			if (!json.ContainsKey("data")) {
				return false;
			}
			return true;
		}
	}
}