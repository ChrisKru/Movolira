using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Movolira {
	public class MovieDataProvider {
		public Dictionary<string, JObject> HttpCache { get; }
		public string TMDBImagesBaseUrl { get; private set; }
		public string TMDBBackdropSmallSize { get; private set; }
		public string TMDBBackdropSize { get; private set; }
		public string TMDBPosterSize { get; private set; }

		[JsonConstructor]
		public MovieDataProvider(Dictionary<string, JObject> HttpCache, string TMDBImagesBaseUrl, string TMDBBackdropSmallSize,
		                         string TMDBBackdropSize, string TMDBPosterSize) {
			this.HttpCache = HttpCache;
			this.TMDBImagesBaseUrl = TMDBImagesBaseUrl;
			this.TMDBBackdropSmallSize = TMDBBackdropSmallSize;
			this.TMDBBackdropSize = TMDBBackdropSize;
			this.TMDBPosterSize = TMDBPosterSize;
		}

		public MovieDataProvider() {
			HttpCache = new Dictionary<string, JObject>();
			setupTMDBImages();
		}

		private void setupTMDBImages() {
			HttpClient http_client = new HttpClient();
			http_client.MaxResponseContentBufferSize = 256000;
			Uri setup_uri = new Uri("https://api.themoviedb.org/3/configuration?api_key=" + ApiKeys.TMDB_KEY);
			HttpResponseMessage setup_response = http_client.GetAsync(setup_uri).Result;
			if (setup_response.IsSuccessStatusCode) {
				string setup_data = setup_response.Content.ReadAsStringAsync().Result;
				JObject setup_json = JObject.Parse(setup_data);
				JToken setup_images = setup_json["images"];
				TMDBImagesBaseUrl = setup_images["base_url"].Value<string>();
				IList<JToken> backdrop_sizes = setup_images["backdrop_sizes"].Children().ToList();
				TMDBBackdropSmallSize = backdrop_sizes[1].Value<string>();
				TMDBBackdropSize = backdrop_sizes[1].Value<string>();
				IList<JToken> poster_sizes = setup_images["poster_sizes"].Children().ToList();
				TMDBPosterSize = poster_sizes[0].Value<string>();
			}
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
				Movie movie = new Movie(trakt_id, tmdb_id, title, genres, release_date, runtime, rating, votes, certification, overview);
				movies.Add(movie);
			}
			return movies;
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
				Uri popular_movies_uri = new Uri("https://api.trakt.tv/movies/popular?extended=full");
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

		public void getMovieImages(Movie movie) {
			JObject images_json = getMovieImagesJson(movie.TMDB_ID);
			string poster_file_path = images_json["posters"].Children().First()["file_path"].Value<string>();
			string backdrop_file_path = images_json["backdrops"].Children().First()["file_path"].Value<string>();
			movie.setImages(poster_file_path, backdrop_file_path);
		}

		private JObject getMovieImagesJson(string movie_tmdb_id) {
			JObject images_json;
			if (!HttpCache.TryGetValue("images" + movie_tmdb_id, out images_json)) {
				HttpClient http_client = new HttpClient();
				http_client.MaxResponseContentBufferSize = 256000;
				Uri images_uri = new Uri("https://api.themoviedb.org/3/movie/" + movie_tmdb_id + "/images?api_key=" + ApiKeys.TMDB_KEY);
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

		public string getMoviePosterUrl(string poster_file_name) {
			return TMDBImagesBaseUrl + TMDBPosterSize + "/" + poster_file_name;
		}

		public string getMovieBackdropUrl(string backdrop_file_name) {
			return TMDBImagesBaseUrl + TMDBBackdropSize + "/" + backdrop_file_name;
		}
	}
}