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


		[JsonConstructor]
		public DataProvider() {
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

		public async Task<Tuple<List<Movie>, int>> getTrendingMovies(int page_number) {
			Uri trending_movies_uri = new Uri("https://api.trakt.tv/movies/trending?extended=full&limit=" + SHOWS_PER_PAGE + "&page=" + page_number);
			List<Movie> movies = null;
			JObject movies_json = await getMoviesJson(page_number, "trending", trending_movies_uri);
			var images_loading_tasks = new List<Task>();
			if (movies_json == null) {
				return null;
			}
			if (!movies_json.ContainsKey("data") || !movies_json.ContainsKey("page_count") || !movies_json.ContainsKey("page_item_count")) {
				return null;
			}
			movies = new List<Movie>();
			IList<JToken> movies_jtokens = movies_json["data"].Children().ToList();
			foreach (JToken movie_jtoken in movies_jtokens) {
				string trakt_id = movie_jtoken["movie"]["ids"]["trakt"].Value<string>();
				string tmdb_id = movie_jtoken["movie"]["ids"]["tmdb"].Value<string>();
				string title = movie_jtoken["movie"]["title"].Value<string>();
				var genres = movie_jtoken["movie"]["genres"].Select(genre => (string) genre).ToArray();
				string release_date = movie_jtoken["movie"]["released"].Value<string>();
				int runtime = movie_jtoken["movie"]["runtime"].Value<int>();
				double rating = movie_jtoken["movie"]["rating"].Value<double>();
				int votes = movie_jtoken["movie"]["votes"].Value<int>();
				string certification = movie_jtoken["movie"]["certification"].Value<string>();
				string overview = movie_jtoken["movie"]["overview"].Value<string>();
				Movie movie = new Movie(trakt_id, tmdb_id, title, genres, release_date, runtime, rating, votes, certification, overview);
				movies.Add(movie);
				images_loading_tasks.Add(getMovieImages(movie));
			}
			int page_count = movies_json["page_count"].Value<int>();
			int page_item_count = movies_json["page_item_count"].Value<int>();
			int item_count = page_count * page_item_count;
			await Task.WhenAll(images_loading_tasks);
			return Tuple.Create(movies, item_count);
		}

		public async Task<Tuple<List<Movie>, int>> getMostPopularMovies(int page_number) {
			Uri popular_movies_uri = new Uri("https://api.trakt.tv/movies/popular?extended=full&limit=" + SHOWS_PER_PAGE + "&page=" + page_number);
			List<Movie> movies = null;
			JObject movies_json = await getMoviesJson(page_number, "most_popular", popular_movies_uri);
			var images_loading_tasks = new List<Task>();
			if (movies_json == null) {
				return null;
			}
			if (!movies_json.ContainsKey("data") || !movies_json.ContainsKey("page_count") || !movies_json.ContainsKey("page_item_count")) {
				return null;
			}
			movies = new List<Movie>();
			IList<JToken> movies_jtokens = movies_json["data"].Children().ToList();
			foreach (JToken movie_jtoken in movies_jtokens) {
				string trakt_id = movie_jtoken["ids"]["trakt"].Value<string>();
				string tmdb_id = movie_jtoken["ids"]["tmdb"].Value<string>();
				string title = movie_jtoken["title"].Value<string>();
				var genres = movie_jtoken["genres"].Select(genre => (string) genre).ToArray();
				string release_date = movie_jtoken["released"].Value<string>();
				int runtime = movie_jtoken["runtime"].Value<int>();
				double rating = movie_jtoken["rating"].Value<double>();
				int votes = movie_jtoken["votes"].Value<int>();
				string certification = movie_jtoken["certification"].Value<string>();
				string overview = movie_jtoken["overview"].Value<string>();
				Movie movie = new Movie(trakt_id, tmdb_id, title, genres, release_date, runtime, rating, votes, certification, overview);
				movies.Add(movie);
				images_loading_tasks.Add(getMovieImages(movie));
			}
			int page_count = movies_json["page_count"].Value<int>();
			int page_item_count = movies_json["page_item_count"].Value<int>();
			int item_count = page_count * page_item_count;
			await Task.WhenAll(images_loading_tasks);
			return Tuple.Create(movies, item_count);
		}

		public async Task<Tuple<List<Movie>, int>> getMostWatchedMovies(int page_number) {
			Uri trending_movies_uri = new Uri("https://api.trakt.tv/movies/watched?extended=full&limit=" + SHOWS_PER_PAGE + "&page=" + page_number);
			List<Movie> movies = null;
			JObject movies_json = await getMoviesJson(page_number, "most_watched", trending_movies_uri);
			var images_loading_tasks = new List<Task>();
			if (movies_json == null) {
				return null;
			}
			if (!movies_json.ContainsKey("data") || !movies_json.ContainsKey("page_count") || !movies_json.ContainsKey("page_item_count")) {
				return null;
			}
			movies = new List<Movie>();
			IList<JToken> movies_jtokens = movies_json["data"].Children().ToList();
			foreach (JToken movie_jtoken in movies_jtokens) {
				string trakt_id = movie_jtoken["movie"]["ids"]["trakt"].Value<string>();
				string tmdb_id = movie_jtoken["movie"]["ids"]["tmdb"].Value<string>();
				string title = movie_jtoken["movie"]["title"].Value<string>();
				var genres = movie_jtoken["movie"]["genres"].Select(genre => (string) genre).ToArray();
				string release_date = movie_jtoken["movie"]["released"].Value<string>();
				int runtime = movie_jtoken["movie"]["runtime"].Value<int>();
				double rating = movie_jtoken["movie"]["rating"].Value<double>();
				int votes = movie_jtoken["movie"]["votes"].Value<int>();
				string certification = movie_jtoken["movie"]["certification"].Value<string>();
				string overview = movie_jtoken["movie"]["overview"].Value<string>();
				Movie movie = new Movie(trakt_id, tmdb_id, title, genres, release_date, runtime, rating, votes, certification, overview);
				movies.Add(movie);
				images_loading_tasks.Add(getMovieImages(movie));
			}
			int page_count = movies_json["page_count"].Value<int>();
			int page_item_count = movies_json["page_item_count"].Value<int>();
			int item_count = page_count * page_item_count;
			await Task.WhenAll(images_loading_tasks);
			return Tuple.Create(movies, item_count);
		}

		public async Task<Tuple<List<Movie>, int>> getMostCollectedMovies(int page_number) {
			Uri trending_movies_uri = new Uri("https://api.trakt.tv/movies/collected?extended=full&limit=" + SHOWS_PER_PAGE + "&page=" + page_number);
			List<Movie> movies = null;
			JObject movies_json = await getMoviesJson(page_number, "most_collected", trending_movies_uri);
			var images_loading_tasks = new List<Task>();
			if (movies_json == null) {
				return null;
			}
			if (!movies_json.ContainsKey("data") || !movies_json.ContainsKey("page_count") || !movies_json.ContainsKey("page_item_count")) {
				return null;
			}
			movies = new List<Movie>();
			IList<JToken> movies_jtokens = movies_json["data"].Children().ToList();
			foreach (JToken movie_jtoken in movies_jtokens) {
				string trakt_id = movie_jtoken["movie"]["ids"]["trakt"].Value<string>();
				string tmdb_id = movie_jtoken["movie"]["ids"]["tmdb"].Value<string>();
				string title = movie_jtoken["movie"]["title"].Value<string>();
				var genres = movie_jtoken["movie"]["genres"].Select(genre => (string) genre).ToArray();
				string release_date = movie_jtoken["movie"]["released"].Value<string>();
				int runtime = movie_jtoken["movie"]["runtime"].Value<int>();
				double rating = movie_jtoken["movie"]["rating"].Value<double>();
				int votes = movie_jtoken["movie"]["votes"].Value<int>();
				string certification = movie_jtoken["movie"]["certification"].Value<string>();
				string overview = movie_jtoken["movie"]["overview"].Value<string>();
				Movie movie = new Movie(trakt_id, tmdb_id, title, genres, release_date, runtime, rating, votes, certification, overview);
				movies.Add(movie);
				images_loading_tasks.Add(getMovieImages(movie));
			}
			int page_count = movies_json["page_count"].Value<int>();
			int page_item_count = movies_json["page_item_count"].Value<int>();
			int item_count = page_count * page_item_count;
			await Task.WhenAll(images_loading_tasks);
			return Tuple.Create(movies, item_count);
		}

		public async Task<Tuple<List<Movie>, int>> getMostAnticipatedMovies(int page_number) {
			Uri trending_movies_uri =
				new Uri("https://api.trakt.tv/movies/anticipated?extended=full&limit=" + SHOWS_PER_PAGE + "&page=" + page_number);
			List<Movie> movies = null;
			JObject movies_json = await getMoviesJson(page_number, "most_anticipated", trending_movies_uri);
			var images_loading_tasks = new List<Task>();
			if (movies_json == null) {
				return null;
			}
			if (!movies_json.ContainsKey("data") || !movies_json.ContainsKey("page_count") || !movies_json.ContainsKey("page_item_count")) {
				return null;
			}
			movies = new List<Movie>();
			IList<JToken> movies_jtokens = movies_json["data"].Children().ToList();
			foreach (JToken movie_jtoken in movies_jtokens) {
				string trakt_id = movie_jtoken["movie"]["ids"]["trakt"].Value<string>();
				string tmdb_id = movie_jtoken["movie"]["ids"]["tmdb"].Value<string>();
				string title = movie_jtoken["movie"]["title"].Value<string>();
				var genres = movie_jtoken["movie"]["genres"].Select(genre => (string) genre).ToArray();
				string release_date = movie_jtoken["movie"]["released"].Value<string>();
				int runtime = movie_jtoken["movie"]["runtime"].Value<int>();
				double rating = movie_jtoken["movie"]["rating"].Value<double>();
				int votes = movie_jtoken["movie"]["votes"].Value<int>();
				string certification = movie_jtoken["movie"]["certification"].Value<string>();
				string overview = movie_jtoken["movie"]["overview"].Value<string>();
				Movie movie = new Movie(trakt_id, tmdb_id, title, genres, release_date, runtime, rating, votes, certification, overview);
				movies.Add(movie);
				images_loading_tasks.Add(getMovieImages(movie));
			}
			int page_count = movies_json["page_count"].Value<int>();
			int page_item_count = movies_json["page_item_count"].Value<int>();
			int item_count = page_count * page_item_count;
			await Task.WhenAll(images_loading_tasks);
			return Tuple.Create(movies, item_count);
		}

		public async Task<Tuple<List<Movie>, int>> getBoxOfficeMovies() {
			Uri trending_movies_uri = new Uri("https://api.trakt.tv/movies/boxoffice?extended=full");
			List<Movie> movies = null;
			JObject movies_json = await getMoviesJson(1, "box_office", trending_movies_uri);
			var images_loading_tasks = new List<Task>();
			if (movies_json == null) {
				return null;
			}
			if (!movies_json.ContainsKey("data")) {
				return null;
			}
			movies = new List<Movie>();
			IList<JToken> movies_jtokens = movies_json["data"].Children().ToList();
			foreach (JToken movie_jtoken in movies_jtokens) {
				string trakt_id = movie_jtoken["movie"]["ids"]["trakt"].Value<string>();
				string tmdb_id = movie_jtoken["movie"]["ids"]["tmdb"].Value<string>();
				string title = movie_jtoken["movie"]["title"].Value<string>();
				var genres = movie_jtoken["movie"]["genres"].Select(genre => (string) genre).ToArray();
				string release_date = movie_jtoken["movie"]["released"].Value<string>();
				int runtime = movie_jtoken["movie"]["runtime"].Value<int>();
				double rating = movie_jtoken["movie"]["rating"].Value<double>();
				int votes = movie_jtoken["movie"]["votes"].Value<int>();
				string certification = movie_jtoken["movie"]["certification"].Value<string>();
				string overview = movie_jtoken["movie"]["overview"].Value<string>();
				Movie movie = new Movie(trakt_id, tmdb_id, title, genres, release_date, runtime, rating, votes, certification, overview);
				movies.Add(movie);
				images_loading_tasks.Add(getMovieImages(movie));
			}
			int item_count = 10;
			await Task.WhenAll(images_loading_tasks);
			return Tuple.Create(movies, item_count);
		}

		private async Task<JObject> getMoviesJson(int page_number, string cache_id, Uri movies_uri) {
			JObject movies_json;
			try {
				movies_json = await BlobCache.LocalMachine.GetObject<JObject>(cache_id + page_number);
			} catch (Exception cache_exception) {
				movies_json = new JObject();
				for (int i_retry = 0; i_retry < HTTP_RETRY_COUNT; ++i_retry) {
					HttpResponseMessage movies_response;
					try {
						movies_response = await HTTP_CLIENT.GetAsync(movies_uri);
					} catch (Exception response_exception) {
						await Task.Delay(HTTP_RETRY_DELAY);
						continue;
					}
					if (!movies_response.IsSuccessStatusCode) {
						await Task.Delay(HTTP_RETRY_DELAY);
						continue;
					}
					string movies_data = movies_response.Content.ReadAsStringAsync().Result;
					JArray movies_json_array = JArray.Parse(movies_data);
					movies_json.Add("data", movies_json_array);
					movies_response.Headers.TryGetValues("X-Pagination-Page-Count", out var page_count_header);
					movies_response.Headers.TryGetValues("X-Pagination-Item-Count", out var page_item_count_header);
					string page_count = page_count_header?.FirstOrDefault();
					string page_item_count = page_item_count_header?.FirstOrDefault();
					movies_json.Add("page_count", page_count);
					movies_json.Add("page_item_count", page_item_count);
					movies_response.Dispose();
					await BlobCache.LocalMachine.InsertObject(cache_id + page_number, movies_json, new DateTimeOffset(DateTime.Now.AddDays(1)));
					break;
				}
			}
			return movies_json;
		}

		public async Task getMovieImages(Movie movie) {
			JObject images_json = await getMovieImagesJson(movie.TMDB_ID);
			if (images_json == null) {
				movie.PosterUrl = "";
				movie.BackdropUrl = "";
				return;
			}
			if (images_json.ContainsKey("movieposter")) {
				movie.PosterUrl = images_json["movieposter"].Children().ToList()[0]["url"].Value<string>();
			}
			if (images_json.ContainsKey("moviethumb")) {
				movie.BackdropUrl = images_json["moviethumb"].Children().ToList()[0]["url"].Value<string>();
			}
			if (movie.PosterUrl == null) {
				movie.PosterUrl = "";
			}
			if (movie.BackdropUrl == null) {
				movie.BackdropUrl = "";
			}
		}

		private async Task<JObject> getMovieImagesJson(string movie_tmdb_id) {
			JObject images_json = null;
			try {
				images_json = await BlobCache.LocalMachine.GetObject<JObject>("images" + movie_tmdb_id);
			} catch (Exception cache_exception) {
				Uri images_uri = new Uri("http://webservice.fanart.tv/v3/movies/" + movie_tmdb_id + "?api_key=" + ApiKeys.FANARTTV_KEY);
				for (int i_retry = 0; i_retry < HTTP_RETRY_COUNT; ++i_retry) {
					HttpResponseMessage images_response;
					try {
						images_response = await HTTP_CLIENT.GetAsync(images_uri);
					} catch (Exception exception) {
						await Task.Delay(HTTP_RETRY_DELAY);
						continue;
					}
					if (!images_response.IsSuccessStatusCode) {
						await Task.Delay(HTTP_RETRY_DELAY);
						continue;
					}
					string images_data = images_response.Content.ReadAsStringAsync().Result;
					images_response.Dispose();
					images_json = JObject.Parse(images_data);
					await BlobCache.LocalMachine.InsertObject("images" + movie_tmdb_id, images_json, new DateTimeOffset(DateTime.Now.AddDays(1)));
					break;
				}
			}
			return images_json;
		}
	}
}