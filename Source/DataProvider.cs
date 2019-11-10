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
			JObject movies_json = await getJson("movies_" + category + ";" + page_number, movies_uri);


			if (!doesJsonContainData(movies_json)) {
				return null;
			}


			if (genre_list.Count == 0) {
				await getGenreList();
				if (genre_list.Count == 0) {
					return null;
				}
			}


			var movies = getMovieListFromJson(movies_json);
			int item_count = movies_json["data"]["total_results"].Value<int>();
			return Tuple.Create(movies, item_count);
		}




		private List<Show> getMovieListFromJson(JObject movies_json) {
			var movies = new List<Show>();
			IList<JToken> movies_jtokens = movies_json["data"]["results"].Children().ToList();


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


			return movies;
		}




		public async Task getMovieDetails(Movie movie) {
			Uri release_dates_uri = new Uri("https://api.themoviedb.org/3/movie/" + movie.Id + "/release_dates?api_key=" + ApiKeys.TMDB_KEY);
			var release_dates_task = getJson("movie_release_dates;" + movie.Id, release_dates_uri);
			Uri details_uri = new Uri("https://api.themoviedb.org/3/movie/" + movie.Id + "?api_key=" + ApiKeys.TMDB_KEY);
			JObject details_json = await getJson("movie;" + movie.Id, details_uri);
			if (!doesJsonContainData(details_json)) {
				return;
			}


			JToken details_json_data = details_json["data"];
			if (details_json_data["runtime"].Type != JTokenType.Null) {
				movie.Runtime = details_json_data["runtime"].Value<int>();
			}


			JObject release_dates_json = await release_dates_task;
			if (!doesJsonContainData(release_dates_json)) {
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




		public async Task<Tuple<List<Show>, int>> getTvShows(string category, int page_number) {
			Uri tv_shows_uri = new Uri("https://api.themoviedb.org/3/tv/" + category + "?api_key=" + ApiKeys.TMDB_KEY + "&page=" + page_number);
			JObject tv_shows_json = await getJson("tv_shows_" + category + ";" + page_number, tv_shows_uri);


			if (!doesJsonContainData(tv_shows_json)) {
				return null;
			}


			if (genre_list.Count == 0) {
				await getGenreList();
				if (genre_list.Count == 0) {
					return null;
				}
			}


			var tv_shows = getTvShowListFromJson(tv_shows_json);
			int item_count = tv_shows_json["data"]["total_results"].Value<int>();
			return Tuple.Create(tv_shows, item_count);
		}




		private List<Show> getTvShowListFromJson(JObject tv_shows_json) {
			var tv_shows = new List<Show>();
			IList<JToken> tv_shows_jtokens = tv_shows_json["data"]["results"].Children().ToList();


			foreach (JToken tv_show_jtoken in tv_shows_jtokens) {
				string id = tv_show_jtoken["id"].Value<string>();
				string title = tv_show_jtoken["name"].Value<string>();


				IList<JToken> genre_ids = tv_show_jtoken["genre_ids"].Children().ToList();
				var genres = new List<string>();
				foreach (JToken genre_id in genre_ids) {
					string genre = genre_list[genre_id.Value<int>()];
					if (!genre.Contains("&")) {
						genres.Add(genre);
					} else {
						var split_genres = genre.Split(new[] {'&', ' '}, StringSplitOptions.RemoveEmptyEntries);
						foreach (string split_genre in split_genres) {
							genres.Add(split_genre);
						}
					}
				}


				string release_date = tv_show_jtoken["first_air_date"].Value<string>();
				double rating = tv_show_jtoken["vote_average"].Value<double>();
				int votes = tv_show_jtoken["vote_count"].Value<int>();
				string overview = tv_show_jtoken["overview"].Value<string>();


				TvShow tv_show = new TvShow(ShowType.TvShow, id, title, genres.ToArray(), release_date, rating, votes, overview);
				tv_show.PosterUrl = "http://image.tmdb.org/t/p/w500/" + tv_show_jtoken["poster_path"].Value<string>();
				tv_show.BackdropUrl = "http://image.tmdb.org/t/p/w780/" + tv_show_jtoken["backdrop_path"].Value<string>();
				tv_shows.Add(tv_show);
			}


			return tv_shows;
		}




		public async Task getTvShowDetails(TvShow tv_show) {
			Uri content_ratings_uri = new Uri("https://api.themoviedb.org/3/tv/" + tv_show.Id + "/content_ratings?api_key=" + ApiKeys.TMDB_KEY);
			var content_ratings_task = getJson("tv_show_content_ratings;" + tv_show.Id, content_ratings_uri);
			Uri details_uri = new Uri("https://api.themoviedb.org/3/tv/" + tv_show.Id + "?api_key=" + ApiKeys.TMDB_KEY);
			JObject details_json = await getJson("tv_show;" + tv_show.Id, details_uri);
			if (!doesJsonContainData(details_json)) {
				return;
			}


			JToken details_json_data = details_json["data"];
			if (details_json_data["episode_run_time"].Type != JTokenType.Null) {
				IList<JToken> episode_run_times = details_json_data["episode_run_time"].Children().ToList();
				tv_show.Runtime = 0;
				foreach (JToken episode_run_time in episode_run_times) {
					tv_show.Runtime += episode_run_time.Value<int>();
				}
				tv_show.Runtime /= episode_run_times.Count();
			}



			JObject content_ratings_json = await content_ratings_task;
			if (!doesJsonContainData(content_ratings_json)) {
				return;
			}


			IList<JToken> content_ratings_jtokens = content_ratings_json["data"]["results"].Children().ToList();
			foreach (JToken content_ratings_jtoken in content_ratings_jtokens) {
				if (content_ratings_jtoken["iso_3166_1"].Value<string>() == "DE") {
					string certification = content_ratings_jtoken["rating"].Value<string>();
					if (certification != "") {
						tv_show.Certification = certification + "+";
					}
					return;
				}
			}
		}




		private async Task getGenreList() {
			Uri tv_show_genres_uri = new Uri("https://api.themoviedb.org/3/genre/tv/list" + "?api_key=" + ApiKeys.TMDB_KEY);
			var tv_show_genres_task = getJson("tv_genres", tv_show_genres_uri);
			Uri movie_genres_uri = new Uri("https://api.themoviedb.org/3/genre/movie/list" + "?api_key=" + ApiKeys.TMDB_KEY);
			JObject movie_genres_json = await getJson("movie_genres", movie_genres_uri);


			if (!doesJsonContainData(movie_genres_json)) {
				return;
			}


			IList<JToken> movie_genres_jtokens = movie_genres_json["data"]["genres"].Children().ToList();
			foreach (JToken genre_jtoken in movie_genres_jtokens) {
				int id = genre_jtoken["id"].Value<int>();
				string name = genre_jtoken["name"].Value<string>();
				if (!genre_list.ContainsKey(id)) {
					genre_list.Add(id, name);
				}
			}


			JObject tv_show_genres_json = await tv_show_genres_task;
			if (!doesJsonContainData(tv_show_genres_json)) {
				return;
			}


			IList<JToken> tv_show_genres_jtokens = tv_show_genres_json["data"]["genres"].Children().ToList();
			foreach (JToken genre_jtoken in tv_show_genres_jtokens) {
				int id = genre_jtoken["id"].Value<int>();
				string name = genre_jtoken["name"].Value<string>();
				if (!genre_list.ContainsKey(id)) {
					genre_list.Add(id, name);
				}
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




		private bool doesJsonContainData(JObject json) {
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