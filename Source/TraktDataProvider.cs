/*using System;
using System.Collections.Generic;
using System.Globalization;
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


		public async Task<Tuple<List<Show>, int>> getMovies(string category, int page_number, string filter_query) {
			if (category == "popular") {
				return await getMostPopularMovies(page_number, filter_query);
			}
			if (category == "box_office") {
				return await getBoxOfficeMovies();
			}


			Uri movies_uri = new Uri("https://api.trakt.tv/movies/" + category + "?extended=full&limit=" + SHOWS_PER_PAGE + "&page=" + page_number +
			                         "&" + filter_query);
			List<Show> movies = null;
			JObject movies_json = await getShowsJson(page_number, "movies_" + category + "&" + filter_query, movies_uri);


			if (movies_json == null) {
				return null;
			}
			if (!movies_json.ContainsKey("data") || !movies_json.ContainsKey("page_count") || !movies_json.ContainsKey("page_item_count")) {
				return null;
			}


			var images_loading_tasks = new List<Task>();
			movies = new List<Show>();
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


				Movie movie = new Movie(ShowType.Movie, trakt_id, tmdb_id, title, genres, release_date, runtime, rating, votes, certification,
					overview);
				movies.Add(movie);
				images_loading_tasks.Add(getMovieImages(movie));
			}


			int page_count = movies_json["page_count"].Value<int>();
			int page_item_count = movies_json["page_item_count"].Value<int>();
			int item_count = page_count * page_item_count;
			await Task.WhenAll(images_loading_tasks);


			return Tuple.Create(movies, item_count);
		}


		public async Task<Tuple<List<Show>, int>> getMostPopularMovies(int page_number, string filter_query) {
			Uri most_popular_movies_uri = new Uri("https://api.trakt.tv/movies/popular?extended=full&limit=" + SHOWS_PER_PAGE + "&page=" +
			                                      page_number + "&" + filter_query);
			List<Show> movies = null;
			JObject movies_json = await getShowsJson(page_number, "movies_most_popular" + "&" + filter_query, most_popular_movies_uri);


			if (movies_json == null) {
				return null;
			}
			if (!movies_json.ContainsKey("data") || !movies_json.ContainsKey("page_count") || !movies_json.ContainsKey("page_item_count")) {
				return null;
			}


			var images_loading_tasks = new List<Task>();
			movies = new List<Show>();
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


				Movie movie = new Movie(ShowType.Movie, trakt_id, tmdb_id, title, genres, release_date, runtime, rating, votes, certification,
					overview);
				movies.Add(movie);
				images_loading_tasks.Add(getMovieImages(movie));
			}


			int page_count = movies_json["page_count"].Value<int>();
			int page_item_count = movies_json["page_item_count"].Value<int>();
			int item_count = page_count * page_item_count;
			await Task.WhenAll(images_loading_tasks);


			return Tuple.Create(movies, item_count);
		}


		public async Task<Tuple<List<Show>, int>> getBoxOfficeMovies() {
			Uri box_office_movies_uri = new Uri("https://api.trakt.tv/movies/boxoffice?extended=full");
			List<Show> movies = null;
			JObject movies_json = await getShowsJson(1, "box_office", box_office_movies_uri);


			if (movies_json == null) {
				return null;
			}
			if (!movies_json.ContainsKey("data")) {
				return null;
			}


			var images_loading_tasks = new List<Task>();
			movies = new List<Show>();
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


				Movie movie = new Movie(ShowType.Movie, trakt_id, tmdb_id, title, genres, release_date, runtime, rating, votes, certification,
					overview);
				movies.Add(movie);
				images_loading_tasks.Add(getMovieImages(movie));
			}


			int item_count = 10;
			await Task.WhenAll(images_loading_tasks);


			return Tuple.Create(movies, item_count);
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
			} catch (Exception) {
				Uri images_uri = new Uri("http://webservice.fanart.tv/v3/movies/" + movie_tmdb_id + "?api_key=" + ApiKeys.FANARTTV_KEY);
				for (int i_retry = 0; i_retry < HTTP_RETRY_COUNT; ++i_retry) {
					HttpResponseMessage images_response;
					try {
						images_response = await HTTP_CLIENT.GetAsync(images_uri);
					} catch (Exception) {
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


		public async Task<Tuple<List<Show>, int>> getTvShows(string category, int page_number, string filter_query) {
			if (category == "popular") {
				return await getMostPopularTvShows(page_number, filter_query);
			}

			Uri trending_tv_shows_uri = new Uri("https://api.trakt.tv/shows/" + category + "?extended=full&limit=" + SHOWS_PER_PAGE + "&page=" +
			                                    page_number + "&" + filter_query);
			List<Show> tv_shows = null;
			JObject tv_shows_json = await getShowsJson(page_number, "tv_shows_" + category + "&" + filter_query, trending_tv_shows_uri);


			if (tv_shows_json == null) {
				return null;
			}
			if (!tv_shows_json.ContainsKey("data") || !tv_shows_json.ContainsKey("page_count") || !tv_shows_json.ContainsKey("page_item_count")) {
				return null;
			}


			var images_loading_tasks = new List<Task>();
			tv_shows = new List<Show>();
			IList<JToken> tv_shows_jtokens = tv_shows_json["data"].Children().ToList();


			foreach (JToken tv_show_jtoken in tv_shows_jtokens) {
				string trakt_id = tv_show_jtoken["show"]["ids"]["trakt"].Value<string>();
				string tvdb_id = tv_show_jtoken["show"]["ids"]["tvdb"].Value<string>();
				string title = tv_show_jtoken["show"]["title"].Value<string>();
				var genres = tv_show_jtoken["show"]["genres"].Select(genre => (string) genre).ToArray();
				string air_date = tv_show_jtoken["show"]["first_aired"].Value<string>();
				if (air_date != null) {
					air_date = DateTime.Parse(air_date, CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
				}
				int runtime = tv_show_jtoken["show"]["runtime"].Value<int>();
				double rating = tv_show_jtoken["show"]["rating"].Value<double>();
				int votes = tv_show_jtoken["show"]["votes"].Value<int>();
				string certification = tv_show_jtoken["show"]["certification"].Value<string>();
				string overview = tv_show_jtoken["show"]["overview"].Value<string>();


				TvShow tv_show = new TvShow(ShowType.TvShow, trakt_id, tvdb_id, title, genres, air_date, runtime, rating, votes, certification,
					overview);
				tv_shows.Add(tv_show);
				images_loading_tasks.Add(getTvShowImages(tv_show));
			}


			int page_count = tv_shows_json["page_count"].Value<int>();
			int page_item_count = tv_shows_json["page_item_count"].Value<int>();
			int item_count = page_count * page_item_count;
			await Task.WhenAll(images_loading_tasks);


			return Tuple.Create(tv_shows, item_count);
		}


		public async Task<Tuple<List<Show>, int>> getMostPopularTvShows(int page_number, string filter_query) {
			Uri most_popular_tv_shows_uri = new Uri("https://api.trakt.tv/shows/popular?extended=full&limit=" + SHOWS_PER_PAGE + "&page=" +
			                                        page_number + "&" + filter_query);
			List<Show> tv_shows = null;
			JObject tv_shows_json = await getShowsJson(page_number, "tv_shows_most_popular" + "&" + filter_query, most_popular_tv_shows_uri);


			if (tv_shows_json == null) {
				return null;
			}
			if (!tv_shows_json.ContainsKey("data") || !tv_shows_json.ContainsKey("page_count") || !tv_shows_json.ContainsKey("page_item_count")) {
				return null;
			}


			var images_loading_tasks = new List<Task>();
			tv_shows = new List<Show>();
			IList<JToken> tv_shows_jtokens = tv_shows_json["data"].Children().ToList();


			foreach (JToken tv_show_jtoken in tv_shows_jtokens) {
				string trakt_id = tv_show_jtoken["ids"]["trakt"].Value<string>();
				string tvdb_id = tv_show_jtoken["ids"]["tvdb"].Value<string>();
				string title = tv_show_jtoken["title"].Value<string>();
				var genres = tv_show_jtoken["genres"].Select(genre => (string) genre).ToArray();
				string air_date = tv_show_jtoken["first_aired"].Value<string>();
				if (air_date != null) {
					air_date = DateTime.Parse(air_date, CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
				}
				int runtime = tv_show_jtoken["runtime"].Value<int>();
				double rating = tv_show_jtoken["rating"].Value<double>();
				int votes = tv_show_jtoken["votes"].Value<int>();
				string certification = tv_show_jtoken["certification"].Value<string>();
				string overview = tv_show_jtoken["overview"].Value<string>();


				TvShow tv_show = new TvShow(ShowType.TvShow, trakt_id, tvdb_id, title, genres, air_date, runtime, rating, votes, certification,
					overview);
				tv_shows.Add(tv_show);
				images_loading_tasks.Add(getTvShowImages(tv_show));
			}


			int page_count = tv_shows_json["page_count"].Value<int>();
			int page_item_count = tv_shows_json["page_item_count"].Value<int>();
			int item_count = page_count * page_item_count;
			await Task.WhenAll(images_loading_tasks);


			return Tuple.Create(tv_shows, item_count);
		}


		public async Task getTvShowImages(TvShow tv_show) {
			JObject images_json = await getTvShowImagesJson(tv_show.TVDB_ID);
			if (images_json == null) {
				tv_show.PosterUrl = "";
				tv_show.BackdropUrl = "";
				return;
			}


			if (images_json.ContainsKey("tvposter")) {
				tv_show.PosterUrl = images_json["tvposter"].Children().ToList()[0]["url"].Value<string>();
			} else if (images_json.ContainsKey("seasonposter")) {
				tv_show.PosterUrl = images_json["seasonposter"].Children().ToList()[0]["url"].Value<string>();
			}
			if (images_json.ContainsKey("tvthumb")) {
				tv_show.BackdropUrl = images_json["tvthumb"].Children().ToList()[0]["url"].Value<string>();
			} else if (images_json.ContainsKey("seasonthumb")) {
				tv_show.PosterUrl = images_json["seasonthumb"].Children().ToList()[0]["url"].Value<string>();
			}


			if (tv_show.PosterUrl == null) {
				tv_show.PosterUrl = "";
			}
			if (tv_show.BackdropUrl == null) {
				tv_show.BackdropUrl = "";
			}
		}


		private async Task<JObject> getTvShowImagesJson(string tv_show_tmdb_id) {
			JObject images_json = null;
			try {
				images_json = await BlobCache.LocalMachine.GetObject<JObject>("images" + tv_show_tmdb_id);
			} catch (Exception) {
				Uri images_uri = new Uri("http://webservice.fanart.tv/v3/tv/" + tv_show_tmdb_id + "?api_key=" + ApiKeys.FANARTTV_KEY);
				for (int i_retry = 0; i_retry < HTTP_RETRY_COUNT; ++i_retry) {
					HttpResponseMessage images_response;
					try {
						images_response = await HTTP_CLIENT.GetAsync(images_uri);
					} catch (Exception) {
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
					await BlobCache.LocalMachine.InsertObject("images" + tv_show_tmdb_id, images_json, new DateTimeOffset(DateTime.Now.AddDays(1)));
					break;
				}
			}


			return images_json;
		}


		private async Task<JObject> getShowsJson(int page_number, string cache_id, Uri movies_uri) {
			JObject movies_json;
			try {
				movies_json = await BlobCache.LocalMachine.GetObject<JObject>(cache_id + ";" + page_number);
			} catch (Exception) {
				movies_json = new JObject();
				for (int i_retry = 0; i_retry < HTTP_RETRY_COUNT; ++i_retry) {
					HttpResponseMessage movies_response;
					try {
						movies_response = await HTTP_CLIENT.GetAsync(movies_uri);
					} catch (Exception) {
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

		public async Task<Tuple<List<Show>, int>> searchShows(string query, int page_number, string filter_query) {
			List<Show> shows = null;
			JObject search_json = await getSearchJson(page_number, query + "&" + filter_query);


			if (search_json == null) {
				return null;
			}
			if (!search_json.ContainsKey("data") || !search_json.ContainsKey("page_count") || !search_json.ContainsKey("page_item_count")) {
				return null;
			}


			var images_loading_tasks = new List<Task>();
			shows = new List<Show>();
			IList<JToken> search_jtokens = search_json["data"].Children().ToList();
			foreach (JToken search_jtoken in search_jtokens) {
				string type = search_jtoken["type"].Value<string>();
				if (type == "show") {
					string trakt_id = search_jtoken["show"]["ids"]["trakt"].Value<string>();
					string tvdb_id = search_jtoken["show"]["ids"]["tvdb"].Value<string>();
					string title = search_jtoken["show"]["title"].Value<string>();
					var genres = search_jtoken["show"]["genres"].Select(genre => (string) genre).ToArray();
					string air_date = search_jtoken["show"]["first_aired"].Value<string>();
					if (air_date != null) {
						air_date = DateTime.Parse(air_date, CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
					}
					int runtime = search_jtoken["show"]["runtime"].Value<int>();
					double rating = search_jtoken["show"]["rating"].Value<double>();
					int votes = search_jtoken["show"]["votes"].Value<int>();
					string certification = search_jtoken["show"]["certification"].Value<string>();
					string overview = search_jtoken["show"]["overview"].Value<string>();


					TvShow tv_show = new TvShow(ShowType.TvShow, trakt_id, tvdb_id, title, genres, air_date, runtime, rating, votes, certification,
						overview);
					shows.Add(tv_show);
					images_loading_tasks.Add(getTvShowImages(tv_show));
				} else if (type == "movie") {
					string trakt_id = search_jtoken["movie"]["ids"]["trakt"].Value<string>();
					string tmdb_id = search_jtoken["movie"]["ids"]["tmdb"].Value<string>();
					string title = search_jtoken["movie"]["title"].Value<string>();
					var genres = search_jtoken["movie"]["genres"].Select(genre => (string) genre).ToArray();
					string release_date = search_jtoken["movie"]["released"].Value<string>();
					int runtime = search_jtoken["movie"]["runtime"].Value<int>();
					double rating = search_jtoken["movie"]["rating"].Value<double>();
					int votes = search_jtoken["movie"]["votes"].Value<int>();
					string certification = search_jtoken["movie"]["certification"].Value<string>();
					string overview = search_jtoken["movie"]["overview"].Value<string>();


					Movie movie = new Movie(ShowType.Movie, trakt_id, tmdb_id, title, genres, release_date, runtime, rating, votes, certification,
						overview);
					shows.Add(movie);
					images_loading_tasks.Add(getMovieImages(movie));
				}
			}


			int page_count = search_json["page_count"].Value<int>();
			int page_item_count = search_json["page_item_count"].Value<int>();
			int item_count = page_count * page_item_count;
			await Task.WhenAll(images_loading_tasks);


			return Tuple.Create(shows, item_count);
		}

		private async Task<JObject> getSearchJson(int page_number, string query) {
			Uri search_uri = new Uri("https://api.trakt.tv/search/movie,show?extended=full&limit=" + SHOWS_PER_PAGE + "&page=" + page_number +
			                         "&query=" + query);
			JObject search_json;
			try {
				search_json = await BlobCache.LocalMachine.GetObject<JObject>("search" + page_number + ";" + query);
			} catch (Exception) {
				search_json = new JObject();
				for (int i_retry = 0; i_retry < HTTP_RETRY_COUNT; ++i_retry) {
					HttpResponseMessage search_response;
					try {
						search_response = await HTTP_CLIENT.GetAsync(search_uri);
					} catch (Exception) {
						await Task.Delay(HTTP_RETRY_DELAY);
						continue;
					}


					if (!search_response.IsSuccessStatusCode) {
						await Task.Delay(HTTP_RETRY_DELAY);
						continue;
					}


					string search_data = search_response.Content.ReadAsStringAsync().Result;
					JArray search_json_array = JArray.Parse(search_data);
					search_json.Add("data", search_json_array);


					search_response.Headers.TryGetValues("X-Pagination-Page-Count", out var page_count_header);
					search_response.Headers.TryGetValues("X-Pagination-Item-Count", out var page_item_count_header);
					string page_count = page_count_header?.FirstOrDefault();
					string page_item_count = page_item_count_header?.FirstOrDefault();
					search_json.Add("page_count", page_count);
					search_json.Add("page_item_count", page_item_count);


					search_response.Dispose();
					await BlobCache.LocalMachine.InsertObject("search" + page_number + ";" + query, search_json,
						new DateTimeOffset(DateTime.Now.AddDays(1)));
					break;
				}
			}


			return search_json;
		}
	}
}*/