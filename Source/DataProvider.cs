using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Movolira {
	public class DataProvider {
		private static HttpClient HTTP_CLIENT;
		public Dictionary<string, JObject> HttpCache { get; }

		[JsonConstructor]
		public DataProvider(Dictionary<string, JObject> HttpCache) {
			this.HttpCache = HttpCache;
			initHttpClient();
		}

		public DataProvider() {
			HttpCache = new Dictionary<string, JObject>();
			initHttpClient();
		}

		private void initHttpClient() {
			if (HTTP_CLIENT == null) {
				HTTP_CLIENT = new HttpClient();
				HTTP_CLIENT.MaxResponseContentBufferSize = 256000;
				HTTP_CLIENT.DefaultRequestHeaders.Add("trakt-api-version", "2");
				HTTP_CLIENT.DefaultRequestHeaders.Add("trakt-api-key", ApiKeys.TRAKT_ID);
			}
		}

		private JObject getMoviesJson(int page_number, string cache_id, Uri movies_uri) {
			JObject movies_json;
			if (!HttpCache.TryGetValue(cache_id + page_number, out movies_json)) {
				movies_json = new JObject();
				HttpResponseMessage movies_response = HTTP_CLIENT.GetAsync(movies_uri).Result;
				if (movies_response.IsSuccessStatusCode) {
					string movies_data = movies_response.Content.ReadAsStringAsync().Result;
					JArray movies_json_array = JArray.Parse(movies_data);
					movies_json.Add("data", movies_json_array);
					HttpCache.Add(cache_id + page_number, movies_json);
				} else {
					//HANDLE RESPONSE FAILED
					return null;
				}
			}
			return movies_json;
		}

		public List<Movie> getPopularMovies(int page_number) {
			Uri popular_movies_uri = new Uri("https://api.trakt.tv/movies/popular?extended=full&limit=30" + "&page=" + page_number);
			JObject movies_json = getMoviesJson(page_number, "popular", popular_movies_uri);
			var movies = new List<Movie>();
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
			}
			return movies;
		}

		public List<Movie> getTrendingMovies(int page_number) {
			Uri trending_movies_uri = new Uri("https://api.trakt.tv/movies/trending?extended=full&limit=30" + "&page=" + page_number);
			JObject movies_json = getMoviesJson(page_number, "trending", trending_movies_uri);
			var movies = new List<Movie>();
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
			}
			return movies;
		}

		public void getMovieImages(Movie movie) {
			JObject images_json = getMovieImagesJson(movie.TMDB_ID);
			movie.PosterUrl = images_json["movieposter"].Children().ToList()[0]["url"].Value<string>();
			movie.BackdropUrl = images_json["moviethumb"].Children().ToList()[0]["url"].Value<string>();
		}

		private JObject getMovieImagesJson(string movie_tmdb_id) {
			JObject images_json;
			if (!HttpCache.TryGetValue("images" + movie_tmdb_id, out images_json)) {
				Uri images_uri = new Uri("http://webservice.fanart.tv/v3/movies/" + movie_tmdb_id + "?api_key=" + ApiKeys.FANARTTV_KEY);
				HttpResponseMessage images_response = HTTP_CLIENT.GetAsync(images_uri).Result;
				if (images_response.IsSuccessStatusCode) {
					string images_data = images_response.Content.ReadAsStringAsync().Result;
					images_json = JObject.Parse(images_data);
					HttpCache["images" + movie_tmdb_id] = images_json;
				} else {
					//HANDLE RESPONSE FAILED
					return null;
				}
			}
			return images_json;
		}
	}
}