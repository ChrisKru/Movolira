using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Movolira {
	public class MovieDataProvider {
		public string TMDBImagesBaseUrl { get; private set; }
		public string TMDBBackdropItemSize { get; private set; }
		public string TMDBBackdropSize { get; private set; }
		public string TMDBPosterSize { get; private set; }
		public Dictionary<int, string> TMDBGenres { get; private set; }
		public Dictionary<string, JObject> HttpCache { get; }

		[JsonConstructor]
		public MovieDataProvider(string TMDBImagesBaseUrl, string TMDBBackdropItemSize, string TMDBBackdropSize, string TMDBPosterSize, Dictionary<int, string> TMDBGenres,
		                         Dictionary<string, JObject> HttpCache) {
			this.TMDBImagesBaseUrl = TMDBImagesBaseUrl;
			this.TMDBBackdropItemSize = TMDBBackdropItemSize;
			this.TMDBBackdropSize = TMDBBackdropSize;
			this.TMDBPosterSize = TMDBPosterSize;
			this.TMDBGenres = TMDBGenres;
			this.HttpCache = HttpCache;
		}

		public MovieDataProvider() {
			HttpCache = new Dictionary<string, JObject>();
			setupTMDBImages();
			setupTMDBGenres();
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
				if (backdrop_sizes.Count == 1) {
					TMDBBackdropItemSize = backdrop_sizes[0].Value<string>();
					TMDBBackdropSize = backdrop_sizes[0].Value<string>();
				} else {
					TMDBBackdropItemSize = backdrop_sizes[backdrop_sizes.Count / 2 - 1].Value<string>();
					TMDBBackdropSize = backdrop_sizes[backdrop_sizes.Count - 1].Value<string>();
				}
				IList<JToken> poster_sizes = setup_images["poster_sizes"].Children().ToList();
				if (poster_sizes.Count == 1) {
					TMDBPosterSize = poster_sizes[0].Value<string>();
				} else {
					TMDBPosterSize = poster_sizes[poster_sizes.Count / 2].Value<string>();
				}
			}
		}

		private void setupTMDBGenres() {
			HttpClient http_client = new HttpClient();
			http_client.MaxResponseContentBufferSize = 256000;
			Uri genres_uri = new Uri("https://api.themoviedb.org/3/genre/movie/list?api_key=" + ApiKeys.TMDB_KEY);
			HttpResponseMessage genres_response = http_client.GetAsync(genres_uri).Result;
			if (genres_response.IsSuccessStatusCode) {
				string genres_data = genres_response.Content.ReadAsStringAsync().Result;
				JObject genres_json = JObject.Parse(genres_data);
				IList<JToken> genres = genres_json["genres"].Children().ToList();
				TMDBGenres = new Dictionary<int, string>();
				foreach (JToken genre in genres) {
					TMDBGenres.Add(genre["id"].Value<int>(), genre["name"].Value<string>());
				}
			}
		}

		public List<MovieCard> getPopularMovieCards() {
			JObject popular_movies_json = getPopularMoviesJson();
			IList<JToken> popular_movies = popular_movies_json["results"].Children().ToList();
			var popular_movie_cards = new List<MovieCard>();
			foreach (JToken movie in popular_movies) {
				int id = movie["id"].Value<int>();
				string backdrop_item_path = TMDBImagesBaseUrl + TMDBBackdropItemSize + movie["backdrop_path"].Value<string>();
				string backdrop_path = TMDBImagesBaseUrl + TMDBBackdropSize + movie["backdrop_path"].Value<string>();
				string poster_path = TMDBImagesBaseUrl + TMDBPosterSize + movie["poster_path"].Value<string>();
				string title = movie["title"].Value<string>();
				string overview = movie["overview"].Value<string>();
				string release_date = movie["release_date"].Value<string>();
				double rating = movie["vote_average"].Value<double>();
				string genre_text = "";
				IList<int> genre_ids = movie["genre_ids"].Select(x => (int) x).ToList();
				for (int i_genre_ids = 0; i_genre_ids < genre_ids.Count; ++i_genre_ids) {
					genre_text += TMDBGenres[genre_ids[i_genre_ids]];
					if (i_genre_ids + 1 < genre_ids.Count) {
						genre_text += ", ";
					}
				}
				MovieCard card_movie = new MovieCard(id, backdrop_item_path, backdrop_path, poster_path, title, overview, genre_text, release_date, rating);
				popular_movie_cards.Add(card_movie);
			}
			return popular_movie_cards;
		}

		private JObject getPopularMoviesJson() {
			JObject popular_movies_json;
			if (!HttpCache.TryGetValue("popular", out popular_movies_json)) {
				HttpClient http_client = new HttpClient();
				http_client.MaxResponseContentBufferSize = 256000;
				Uri popular_movies_uri = new Uri("https://api.themoviedb.org/3/discover/movie?api_key=" + ApiKeys.TMDB_KEY + "&sort_by=popularity.desc");
				HttpResponseMessage popular_movies_response = http_client.GetAsync(popular_movies_uri).Result;
				if (popular_movies_response.IsSuccessStatusCode) {
					string popular_movies_data = popular_movies_response.Content.ReadAsStringAsync().Result;
					popular_movies_json = JObject.Parse(popular_movies_data);
					HttpCache.Add("popular", popular_movies_json);
				} else {
					//HANDLE RESPONSE FAILED
					return null;
				}
			}
			return popular_movies_json;
		}

		public DetailedMovie getDetailedMovie(int movie_id) {
			JObject movie_json = getMovieDetailsJson(movie_id);
			string backdrop_item_path = TMDBImagesBaseUrl + TMDBBackdropItemSize + movie_json["backdrop_path"].Value<string>();
			string backdrop_path = TMDBImagesBaseUrl + TMDBBackdropSize + movie_json["backdrop_path"].Value<string>();
			string poster_path = TMDBImagesBaseUrl + TMDBPosterSize + movie_json["poster_path"].Value<string>();
			string title = movie_json["title"].Value<string>();
			string overview = movie_json["overview"].Value<string>();
			string release_date = movie_json["release_date"].Value<string>();
			double rating = movie_json["vote_average"].Value<double>();
			int runtime_min = movie_json["runtime"].Value<int>();
			int runtime_hours = 0;
			while (runtime_min >= 60) {
				++runtime_hours;
				runtime_min -= 60;
			}
			string runtime_text = runtime_hours + "h " + runtime_min + "min";
			string genre_text = "";
			IList<JToken> json_genres = movie_json["genres"].Children().ToList();
			for (int i_genres = 0; i_genres < json_genres.Count; ++i_genres) {
				int genre_id = json_genres[i_genres]["id"].Value<int>();
				genre_text += TMDBGenres[genre_id];
				if (i_genres + 1 < json_genres.Count) {
					genre_text += ", ";
				}
			}
			DetailedMovie detailed_movie = new DetailedMovie(backdrop_item_path, backdrop_path, poster_path, title, overview, genre_text, release_date, rating, runtime_text);
			return detailed_movie;
		}

		private JObject getMovieDetailsJson(int movie_id) {
			JObject movie_json;
			if (!HttpCache.TryGetValue("movie" + movie_id, out movie_json)) {
				HttpClient http_client = new HttpClient();
				http_client.MaxResponseContentBufferSize = 256000;
				Uri movie_uri = new Uri("https://api.themoviedb.org/3/movie/" + movie_id + "?api_key=" + ApiKeys.TMDB_KEY + "&sort_by=popularity.desc");
				HttpResponseMessage movie_response = http_client.GetAsync(movie_uri).Result;
				if (movie_response.IsSuccessStatusCode) {
					string movie_data = movie_response.Content.ReadAsStringAsync().Result;
					movie_json = JObject.Parse(movie_data);
					HttpCache.Add("movie" + movie_id, movie_json);
				} else {
					//HANDLE RESPONSE FAILED
					return null;
				}
			}
			return movie_json;
		}
	}
}