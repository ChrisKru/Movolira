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

		public List<Movie> getPopularMovies(int page_number) {
			return getMoviesFromJson(getPopularMoviesJson(page_number));
		}

		private JObject getPopularMoviesJson(int page_number) {
			JObject popular_movies_json;
			if (!HttpCache.TryGetValue("popular" + page_number, out popular_movies_json)) {
				popular_movies_json = new JObject();
				Uri popular_movies_uri = new Uri("https://api.trakt.tv/movies/popular?extended=full&limit=30" + "&page=" + page_number);
				HttpResponseMessage popular_movies_response = HTTP_CLIENT.GetAsync(popular_movies_uri).Result;
				if (popular_movies_response.IsSuccessStatusCode) {
					string popular_movies_data = popular_movies_response.Content.ReadAsStringAsync().Result;
					JArray popular_movies_json_array = JArray.Parse(popular_movies_data);
					popular_movies_json.Add("data", popular_movies_json_array);
					HttpCache.Add("popular" + page_number, popular_movies_json);
				} else {
					//HANDLE RESPONSE FAILED
					return null;
				}
			}
			return popular_movies_json;
		}

		private List<Movie> getMoviesFromJson(JObject movies_json) {
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
				//JObject images_json = getMovieImagesJson(tmdb_id);
				//string poster_url = images_json["movieposter"].Children().ToList()[0]["url"].Value<string>();
				//string backdrop_url = images_json["moviethumb"].Children().ToList()[0]["url"].Value<string>();
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