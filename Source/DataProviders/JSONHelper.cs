using System;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using Newtonsoft.Json.Linq;




namespace Movolira.DataProviders {
	public static class JSONHelper {
		private const int HTTP_RETRY_COUNT = 5;
		private const int HTTP_RETRY_DELAY = 500; // milliseconds




		public static async Task<JObject> getJson(string cache_id, Uri json_uri) {
			JObject json;
			try {
				json = await BlobCache.LocalMachine.GetObject<JObject>(cache_id);
			} catch (Exception) {
				json = new JObject();


				for (int i_retry = 0; i_retry < HTTP_RETRY_COUNT; ++i_retry) {
					HttpResponseMessage json_response;
					try {
						json_response = await DataProvider.HTTP_CLIENT.GetAsync(json_uri);
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