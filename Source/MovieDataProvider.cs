using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Movolira {
	public class MovieDataProvider {
		public Dictionary<string, JObject> HttpCache { get; }

		[JsonConstructor]
		public MovieDataProvider(Dictionary<string, JObject> HttpCache) {
			this.HttpCache = HttpCache;
		}

		public MovieDataProvider() {
			HttpCache = new Dictionary<string, JObject>();
		}

		public List<Movie> getPopularMovies() {
			return getMoviesFromJson(getPopularMoviesJson());
		}

		private JObject getPopularMoviesJson() {
			JObject popular_movies_json;
			if (!HttpCache.TryGetValue("popular", out popular_movies_json)) {
				popular_movies_json = new JObject();
				HttpClient http_client = new HttpClient();
				http_client.MaxResponseContentBufferSize = 256000;
				http_client.DefaultRequestHeaders.Add("trakt-api-version", "2");
				http_client.DefaultRequestHeaders.Add("trakt-api-key", ApiKeys.TRAKT_ID);
				Uri popular_movies_uri = new Uri("https://api.trakt.tv/movies/popular?extended=full&limit=30");
				HttpResponseMessage popular_movies_response = http_client.GetAsync(popular_movies_uri).Result;
				if (popular_movies_response.IsSuccessStatusCode) {
					string popular_movies_data = popular_movies_response.Content.ReadAsStringAsync().Result;
					JArray popular_movies_json_array = JArray.Parse(popular_movies_data);
					popular_movies_json.Add("data", popular_movies_json_array);
					HttpCache.Add("popular", popular_movies_json);
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
				JObject images_json = getMovieImagesJson(tmdb_id);
				string poster_url = images_json["movieposter"].Children().ToList()[0]["url"].Value<string>();
				string backdrop_url = images_json["moviethumb"].Children().ToList()[0]["url"].Value<string>();
				Movie movie = new Movie(trakt_id, tmdb_id, title, genres, release_date, runtime, rating, votes, certification, overview, poster_url,
					backdrop_url);
				movies.Add(movie);
			}
			return movies;
		}

		private JObject getMovieImagesJson(string movie_tmdb_id) {
			JObject images_json;
			if (!HttpCache.TryGetValue("images" + movie_tmdb_id, out images_json)) {
				HttpClient http_client = new HttpClient();
				http_client.MaxResponseContentBufferSize = 256000;
				Uri images_uri = new Uri("http://webservice.fanart.tv/v3/movies/" + movie_tmdb_id + "?api_key=" + ApiKeys.FANARTTV_KEY);
				HttpResponseMessage images_response = http_client.GetAsync(images_uri).Result;
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