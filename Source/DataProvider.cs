using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Movolira {
	public class DataProvider {
		public const int SHOWS_PER_PAGE = 30;
		private const int HTTP_RETRY_COUNT = 5;
		private const int HTTP_RETRY_DELAY = 500;


		private static HttpClient HTTP_CLIENT;


		private readonly Dictionary<int, string> genre_list;


		[JsonConstructor]
		public DataProvider() {
			genre_list = new Dictionary<int, string>();


			initHttpClient();
			initCache();
		}


		private void initHttpClient() {
			if (HTTP_CLIENT == null) {
				HTTP_CLIENT = new HttpClient();
				HTTP_CLIENT.MaxResponseContentBufferSize = 256000;
				HTTP_CLIENT.DefaultRequestHeaders.Add("trakt-api-version", "2");
				HTTP_CLIENT.DefaultRequestHeaders.Add("trakt-api-key", ApiKeys.TRAKT_ID);
			}
		}


		private void initCache() {
			if (BlobCache.ApplicationName != "Movolira") {
				BlobCache.ApplicationName = "Movolira";
			}
		}


		public async Task<Tuple<List<Show>, int>> getMovies(string category, int page_number) {
			Uri movies_uri = new Uri("https://api.themoviedb.org/3/movie/" + category + "?api_key=" + ApiKeys.TMDB_KEY + "&page=" + page_number);
			List<Show> movies = null;
			JObject movies_json = await getJson("movies_" + category + ";" + page_number, movies_uri);


			if (movies_json == null) {
				return null;
			}
			if (!movies_json.ContainsKey("data")) {
				return null;
			}


			movies = new List<Show>();
			IList<JToken> movies_jtokens = movies_json["data"]["results"].Children().ToList();


			if (genre_list.Count == 0) {
				await getGenreList();
				if (genre_list.Count == 0) {
					return null;
				}
			}


			foreach (JToken movie_jtoken in movies_jtokens) {
				string id = movie_jtoken["id"].Value<string>();
				string title = movie_jtoken["title"].Value<string>();


				IList<JToken> genre_ids = movie_jtoken["genre_ids"].Children().ToList();
				var genres = new List<string>();
				foreach (JToken genre_id in genre_ids) {
					genres.Add(genre_list[genre_id.Value<int>()]);
				}


				string release_date = movie_jtoken["release_date"].Value<string>();
				double rating = movie_jtoken["vote_average"].Value<double>();
				int votes = movie_jtoken["vote_count"].Value<int>();
				string overview = movie_jtoken["overview"].Value<string>();


				Movie movie = new Movie(ShowType.Movie, id, title, genres.ToArray(), release_date, rating, votes, overview);
				movie.PosterUrl = "http://image.tmdb.org/t/p/w500/" + movie_jtoken["poster_path"].Value<string>();
				movie.BackdropUrl = "http://image.tmdb.org/t/p/w780/" + movie_jtoken["backdrop_path"].Value<string>();
				movies.Add(movie);
			}


			int item_count = movies_json["data"]["total_results"].Value<int>();


			return Tuple.Create(movies, item_count);
		}


		public async Task getMovieDetails(Movie movie) {
			Uri release_dates_uri = new Uri("https://api.themoviedb.org/3/movie/" + movie.Id + "/release_dates?api_key=" + ApiKeys.TMDB_KEY);
			var release_dates_task = getJson("movie_release_dates;" + movie.Id, release_dates_uri);
			Uri details_uri = new Uri("https://api.themoviedb.org/3/movie/" + movie.Id + "?api_key=" + ApiKeys.TMDB_KEY);
			JObject details_json = await getJson("movie;" + movie.Id, details_uri);


			if (details_json == null) {
				return;
			}
			if (!details_json.ContainsKey("data")) {
				return;
			}


			JToken details_json_data = details_json["data"];
			if (details_json_data["runtime"].Type != JTokenType.Null) {
				movie.Runtime = details_json_data["runtime"].Value<int>();
			}


			JObject release_dates_json = await release_dates_task;
			if (release_dates_json == null) {
				return;
			}
			if (!release_dates_json.ContainsKey("data")) {
				return;
			}


			IList<JToken> release_dates_jtokens = release_dates_json["data"]["results"].Children().ToList();
			foreach (JToken release_dates_jtoken in release_dates_jtokens) {
				if (release_dates_jtoken["iso_3166_1"].Value<string>() == "DE") {
					string certification = release_dates_jtoken["release_dates"][0]["certification"].Value<string>();
					if (certification != "") {
						movie.Certification = certification + "+";
					}
					return;
				}
			}
		}


		private async Task getGenreList() {
			Uri genres_uri = new Uri("https://api.themoviedb.org/3/genre/movie/list" + "?api_key=" + ApiKeys.TMDB_KEY);
			JObject genres_json = await getJson("genres", genres_uri);


			if (genres_json == null) {
				return;
			}
			if (!genres_json.ContainsKey("data")) {
				return;
			}


			IList<JToken> genres_jtokens = genres_json["data"]["genres"].Children().ToList();
			foreach (JToken genre_jtoken in genres_jtokens) {
				int id = genre_jtoken["id"].Value<int>();
				string name = genre_jtoken["name"].Value<string>();
				genre_list.Add(id, name);
			}
		}


		private async Task<JObject> getJson(string cache_id, Uri json_uri) {
			JObject json;
			try {
				json = await BlobCache.LocalMachine.GetObject<JObject>(cache_id);
			} catch (Exception) {
				json = new JObject();
				for (int i_retry = 0; i_retry < HTTP_RETRY_COUNT; ++i_retry) {
					HttpResponseMessage json_response;
					try {
						json_response = await HTTP_CLIENT.GetAsync(json_uri);
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
					await BlobCache.LocalMachine.InsertObject(cache_id, json, new DateTimeOffset(DateTime.Now.AddDays(1)));
					break;
				}
			}


			return json;
		}
	}
}