﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using Newtonsoft.Json.Linq;




namespace Movolira.DataProviders {
	public class DataProvider {
		public const int SHOWS_PER_PAGE = 20;
		public static HttpClient HTTP_CLIENT;
		private GenresProvider _genres_provider;




		public DataProvider() {
			this.initHttpClient();
			this.initCache();
			this._genres_provider = new GenresProvider();
		}




		private void initHttpClient() {
			if (HTTP_CLIENT == null) {
				HTTP_CLIENT = new HttpClient();
				// Default buffer size results in over 200MB additional taken storage space
				HTTP_CLIENT.MaxResponseContentBufferSize = 256000; // bytes
			}
		}




		private void initCache() {
			if (BlobCache.ApplicationName != "Movolira") {
				BlobCache.ApplicationName = "Movolira";
			}
		}




		public async Task<Dictionary<int, string>> getGenreList() {
			return await this._genres_provider.getGenreList();
		}




		public async Task<Tuple<List<Show>, int>> getMovies(string category, int page_number) {
			Uri movies_uri = new Uri("https://api.themoviedb.org/3/movie/" + category + "?api_key="
				+ ApiKeys.TMDB_KEY + "&page=" + page_number);
			JObject movies_json = await JSONHelper.getJson("movies_" + category + ";" + page_number, movies_uri);
			if (!JSONHelper.doesJsonContainData(movies_json)) {
				return null;
			}


			bool is_genre_list_filled = await this._genres_provider.tryFillGenreList();
			if (!is_genre_list_filled) {
				return null;
			}


			var movies = this.getMovieListFromJson(movies_json);
			int item_count = movies_json["data"]["total_results"].Value<int>();
			return Tuple.Create(movies, item_count);
		}




		private List<Show> getMovieListFromJson(JObject movies_json) {
			var movies = new List<Show>();
			IList<JToken> movies_jtokens = movies_json["data"]["results"].Children().ToList();


			foreach (JToken movie_jtoken in movies_jtokens) {
				string id = "";
				if (JSONHelper.doesJTokenContainKey(movie_jtoken, "id")) {
					id = movie_jtoken["id"].Value<string>();
				}


				string title = "";
				if (JSONHelper.doesJTokenContainKey(movie_jtoken, "title")) {
					title = movie_jtoken["title"].Value<string>();
				}


				var genres = new List<string>();
				if (JSONHelper.doesJTokenContainKey(movie_jtoken, "genre_ids")) {
					IList<JToken> genre_ids = movie_jtoken["genre_ids"].Children().ToList();
					foreach (JToken genre_id in genre_ids) {
						genres.Add(this._genres_provider.getGenreNameForId(genre_id.Value<int>()));
					}
				}
				if (genres.Count == 0) {
					continue;
				}


				string release_date = "";
				if (JSONHelper.doesJTokenContainKey(movie_jtoken, "release_date")) {
					release_date = movie_jtoken["release_date"].Value<string>();
				}


				double rating = 0;
				if (JSONHelper.doesJTokenContainKey(movie_jtoken, "vote_average")) {
					rating = movie_jtoken["vote_average"].Value<double>();
				}


				int vote_count = 0;
				if (JSONHelper.doesJTokenContainKey(movie_jtoken, "vote_count")) {
					vote_count = movie_jtoken["vote_count"].Value<int>();
				}


				string overview = "";
				if (JSONHelper.doesJTokenContainKey(movie_jtoken, "overview")) {
					overview = movie_jtoken["overview"].Value<string>();
				}


				string poster_url = "";
				if (JSONHelper.doesJTokenContainKey(movie_jtoken, "poster_path")) {
					poster_url = "http://image.tmdb.org/t/p/w500/" + movie_jtoken["poster_path"].Value<string>();
				}


				string backdrop_url = "";
				if (JSONHelper.doesJTokenContainKey(movie_jtoken, "backdrop_path")) {
					backdrop_url = "http://image.tmdb.org/t/p/w780/" + movie_jtoken["backdrop_path"].Value<string>();
				}



				Movie movie = new Movie(ShowType.Movie, id, title, genres.ToArray(),
					poster_url, backdrop_url, release_date, rating, vote_count, overview);
				movies.Add(movie);
			}


			return movies;
		}




		public async Task<bool> getMovieDetails(Movie movie) {
			Uri details_uri = new Uri("https://api.themoviedb.org/3/movie/" + movie.Id + "?api_key=" + ApiKeys.TMDB_KEY);
			var details_task = JSONHelper.getJson("movie;" + movie.Id, details_uri);
			Uri release_dates_uri = new Uri("https://api.themoviedb.org/3/movie/" + movie.Id
				+ "/release_dates?api_key=" + ApiKeys.TMDB_KEY);
			var release_dates_task = JSONHelper.getJson("movie_release_dates;" + movie.Id, release_dates_uri);
			JObject details_json = await details_task;


			if (!JSONHelper.doesJsonContainData(details_json)) {
				return false;
			}


			if (movie.Genres == null) {
				var movie_genres = new List<string>();
				if (JSONHelper.doesJTokenContainKey(details_json["data"], "genres")) {
					IList<JToken> genres = details_json["data"]["genres"].Children().ToList();
					foreach (JToken genre in genres) {
						movie_genres.Add(this._genres_provider.getGenreNameForId(genre["id"].Value<int>()));
					}
				}


				movie.Genres = movie_genres.ToArray();
				if (movie.Genres.Length == 0) {
					return false;
				}


				if (JSONHelper.doesJTokenContainKey(details_json["data"], "release_date")) {
					movie.ReleaseDate = details_json["data"]["release_date"].Value<string>();
				}


				if (JSONHelper.doesJTokenContainKey(details_json["data"], "vote_average")) {
					movie.Rating = details_json["data"]["vote_average"].Value<double>();
				}


				if (JSONHelper.doesJTokenContainKey(details_json["data"], "vote_count")) {
					movie.Votes = details_json["data"]["vote_count"].Value<int>();
				}


				if (JSONHelper.doesJTokenContainKey(details_json["data"], "overview")) {
					movie.Overview = details_json["data"]["overview"].Value<string>();
				}


				if (JSONHelper.doesJTokenContainKey(details_json["data"], "poster_path")) {
					movie.PosterUrl = "http://image.tmdb.org/t/p/w500/"
						+ details_json["data"]["poster_path"].Value<string>();
				}


				if (JSONHelper.doesJTokenContainKey(details_json["data"], "backdrop_path")) {
					movie.BackdropUrl = "http://image.tmdb.org/t/p/w780/"
						+ details_json["data"]["backdrop_path"].Value<string>();
				}
			}


			if (JSONHelper.doesJTokenContainKey(details_json["data"], "runtime")) {
				movie.Runtime = details_json["data"]["runtime"].Value<int>();
			}


			JObject release_dates_json = await release_dates_task;
			if (!JSONHelper.doesJsonContainData(release_dates_json)) {
				return true;
			}


			if (JSONHelper.doesJTokenContainKey(release_dates_json["data"], "results")) {
				IList<JToken> release_dates_jtokens = release_dates_json["data"]["results"].Children().ToList();
				foreach (JToken release_dates_jtoken in release_dates_jtokens) {
					if (release_dates_jtoken["iso_3166_1"].Value<string>() == "DE") {
						string certification = release_dates_jtoken["release_dates"][0]["certification"].Value<string>();
						if (certification != "") {
							movie.Certification = certification + "+";
						}
						return true;
					}
				}
			}


			return true;
		}




		public async Task<Tuple<List<Show>, int>> getTvShows(string category, int page_number) {
			Uri tv_shows_uri = new Uri("https://api.themoviedb.org/3/tv/" + category + "?api_key="
				+ ApiKeys.TMDB_KEY + "&page=" + page_number);
			JObject tv_shows_json = await JSONHelper.getJson("tv_shows_" + category + ";" + page_number, tv_shows_uri);


			if (!JSONHelper.doesJsonContainData(tv_shows_json)) {
				return null;
			}


			bool is_genre_list_filled = await this._genres_provider.tryFillGenreList();
			if (!is_genre_list_filled) {
				return null;
			}


			var tv_shows = this.getTvShowListFromJson(tv_shows_json);
			int item_count = tv_shows_json["data"]["total_results"].Value<int>();
			return Tuple.Create(tv_shows, item_count);
		}




		private List<Show> getTvShowListFromJson(JObject tv_shows_json) {
			var tv_shows = new List<Show>();
			IList<JToken> tv_shows_jtokens = tv_shows_json["data"]["results"].Children().ToList();


			foreach (JToken tv_show_jtoken in tv_shows_jtokens) {
				string id = "";
				if (JSONHelper.doesJTokenContainKey(tv_show_jtoken, "id")) {
					id = tv_show_jtoken["id"].Value<string>();
				}


				string title = "";
				if (JSONHelper.doesJTokenContainKey(tv_show_jtoken, "name")) {
					title = tv_show_jtoken["name"].Value<string>();
				}


				IList<JToken> genre_ids = tv_show_jtoken["genre_ids"].Children().ToList();
				var genres = new List<string>();


				foreach (JToken genre_id in genre_ids) {
					string genre = this._genres_provider.getGenreNameForId(genre_id.Value<int>());
					if (!genre.Contains("&")) {
						genres.Add(genre);


					} else {
						var split_genres = genre.Split(new[] { '&', ' ' }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string split_genre in split_genres) {
							genres.Add(split_genre);
						}
					}
				}


				if (genres.Count == 0) {
					continue;
				}


				string release_date = "";
				if (JSONHelper.doesJTokenContainKey(tv_show_jtoken, "first_air_date")) {
					release_date = tv_show_jtoken["first_air_date"].Value<string>();
				}


				double rating = 0;
				if (JSONHelper.doesJTokenContainKey(tv_show_jtoken, "vote_average")) {
					rating = tv_show_jtoken["vote_average"].Value<double>();
				}


				int vote_count = 0;
				if (JSONHelper.doesJTokenContainKey(tv_show_jtoken, "vote_count")) {
					vote_count = tv_show_jtoken["vote_count"].Value<int>();
				}


				string overview = "";
				if (JSONHelper.doesJTokenContainKey(tv_show_jtoken, "overview")) {
					overview = tv_show_jtoken["overview"].Value<string>();
				}


				string poster_url = "";
				if (JSONHelper.doesJTokenContainKey(tv_show_jtoken, "poster_path")) {
					poster_url = "http://image.tmdb.org/t/p/w500/" + tv_show_jtoken["poster_path"].Value<string>();
				}


				string backdrop_url = "";
				if (JSONHelper.doesJTokenContainKey(tv_show_jtoken, "backdrop_path")) {
					backdrop_url = "http://image.tmdb.org/t/p/w780/" + tv_show_jtoken["backdrop_path"].Value<string>();
				}


				TvShow tv_show = new TvShow(ShowType.TvShow, id, title, genres.ToArray(), poster_url,
					backdrop_url, release_date, rating, vote_count, overview);
				tv_shows.Add(tv_show);
			}


			return tv_shows;
		}




		public async Task<bool> getTvShowDetails(TvShow tv_show) {
			Uri details_uri = new Uri("https://api.themoviedb.org/3/tv/" + tv_show.Id + "?api_key=" + ApiKeys.TMDB_KEY);
			var details_task = JSONHelper.getJson("tv_show;" + tv_show.Id, details_uri);
			Uri content_ratings_uri = new Uri("https://api.themoviedb.org/3/tv/"
				+ tv_show.Id + "/content_ratings?api_key=" + ApiKeys.TMDB_KEY);
			var content_ratings_task = JSONHelper.getJson("tv_show_content_ratings;" + tv_show.Id, content_ratings_uri);



			JObject details_json = await details_task;
			if (!JSONHelper.doesJsonContainData(details_json)) {
				return false;
			}


			if (tv_show.Genres == null) {
				var tv_show_genres = new List<string>();
				if (JSONHelper.doesJTokenContainKey(details_json["data"], "genres")) {
					IList<JToken> genres = details_json["data"]["genres"].Children().ToList();
					foreach (JToken genre in genres) {
						tv_show_genres.Add(this._genres_provider.getGenreNameForId(genre["id"].Value<int>()));
					}
				}


				tv_show.Genres = tv_show_genres.ToArray();
				if (tv_show.Genres.Length == 0) {
					return false;
				}


				if (JSONHelper.doesJTokenContainKey(details_json["data"], "first_air_date")) {
					tv_show.AirDate = details_json["data"]["first_air_date"].Value<string>();
				}


				if (JSONHelper.doesJTokenContainKey(details_json["data"], "vote_average")) {
					tv_show.Rating = details_json["data"]["vote_average"].Value<double>();
				}


				if (JSONHelper.doesJTokenContainKey(details_json["data"], "vote_count")) {
					tv_show.Votes = details_json["data"]["vote_count"].Value<int>();
				}


				if (JSONHelper.doesJTokenContainKey(details_json["data"], "overview")) {
					tv_show.Overview = details_json["data"]["overview"].Value<string>();
				}


				if (JSONHelper.doesJTokenContainKey(details_json["data"], "poster_path")) {
					tv_show.PosterUrl = "http://image.tmdb.org/t/p/w500/"
						+ details_json["data"]["poster_path"].Value<string>();
				}


				if (JSONHelper.doesJTokenContainKey(details_json["data"], "backdrop_path")) {
					tv_show.BackdropUrl = "http://image.tmdb.org/t/p/w780/"
						+ details_json["data"]["backdrop_path"].Value<string>();
				}
			}


			if (JSONHelper.doesJTokenContainKey(details_json["data"], "episode_run_time")) {
				if (details_json["data"]["episode_run_time"].Type != JTokenType.Null) {
					IList<JToken> episode_run_times = details_json["data"]["episode_run_time"].Children().ToList();
					tv_show.Runtime = 0;
					if (episode_run_times.Count > 0) {
						foreach (JToken episode_run_time in episode_run_times) {
							tv_show.Runtime += episode_run_time.Value<int>();
						}
						tv_show.Runtime /= episode_run_times.Count();
					}
				}
			}


			JObject content_ratings_json = await content_ratings_task;
			if (!JSONHelper.doesJsonContainData(content_ratings_json)) {
				return true;
			}


			if (JSONHelper.doesJTokenContainKey(content_ratings_json["data"], "results")) {
				IList<JToken> content_ratings_jtokens = content_ratings_json["data"]["results"].Children().ToList();
				foreach (JToken content_ratings_jtoken in content_ratings_jtokens) {
					if (content_ratings_jtoken["iso_3166_1"].Value<string>() == "DE") {
						string certification = content_ratings_jtoken["rating"].Value<string>();
						if (certification != "") {
							tv_show.Certification = certification + "+";
						}
						return true;
					}
				}
			}


			return true;
		}




		public async Task<Tuple<List<Show>, int>> getSearchedShows(string query, int page_number) {
			Uri movies_uri = new Uri("https://api.themoviedb.org/3/search/movie" + "?api_key="
				+ ApiKeys.TMDB_KEY + "&page=" + page_number + "&query=" + query);
			var movies_json_task = JSONHelper.getJson("search_movies_" + query + ";" + page_number, movies_uri);
			Uri tv_shows_uri = new Uri("https://api.themoviedb.org/3/search/tv" + "?api_key="
				+ ApiKeys.TMDB_KEY + "&page=" + page_number + "&query=" + query);
			var tv_shows_json_task = JSONHelper.getJson("search_tv_shows_" + query + ";" + page_number, tv_shows_uri);


			bool is_genre_list_filled = await this._genres_provider.tryFillGenreList();
			if (!is_genre_list_filled) {
				return null;
			}


			JObject movies_json = await movies_json_task;
			if (!JSONHelper.doesJsonContainData(movies_json)) {
				return null;
			}
			var movies = this.getMovieListFromJson(movies_json);
			int movies_max_item_count = movies_json["data"]["total_results"].Value<int>();


			JObject tv_shows_json = await tv_shows_json_task;
			if (!JSONHelper.doesJsonContainData(tv_shows_json)) {
				return null;
			}
			var tv_shows = this.getTvShowListFromJson(tv_shows_json);
			int tv_shows_max_item_count = tv_shows_json["data"]["total_results"].Value<int>();


			List<Show> searched_shows;
			if (movies.Count > tv_shows.Count) {
				searched_shows = movies;
				for (int i_tv_show = 0; i_tv_show < tv_shows.Count; ++i_tv_show) {
					searched_shows.Insert(i_tv_show + 1, tv_shows[i_tv_show]);
				}


			} else {
				searched_shows = tv_shows;
				for (int i_movie = 0; i_movie < movies.Count; ++i_movie) {
					searched_shows.Insert(i_movie + 1, movies[i_movie]);
				}
			}


			int max_item_count;
			if (searched_shows.Count < SHOWS_PER_PAGE) {
				max_item_count = page_number * SHOWS_PER_PAGE;
			} else {
				max_item_count = Math.Max(movies_max_item_count, tv_shows_max_item_count);
			}


			return Tuple.Create(searched_shows, max_item_count);
		}




		public async Task<Tuple<List<Show>, int>> getDiscoveredShows(string query, int page_number) {
			Uri movies_uri = new Uri("https://api.themoviedb.org/3/discover/movie" + "?api_key="
				+ ApiKeys.TMDB_KEY + "&page=" + page_number + "&" + query);
			var movies_json_task = JSONHelper.getJson("discover_movies_" + query + ";" + page_number, movies_uri);
			Uri tv_shows_uri = new Uri("https://api.themoviedb.org/3/discover/tv" + "?api_key="
				+ ApiKeys.TMDB_KEY + "&page=" + page_number + "&" + query);
			var tv_shows_json_task = JSONHelper.getJson("discover_tv_shows_" + query + ";" + page_number, tv_shows_uri);


			bool is_genre_list_filled = await this._genres_provider.tryFillGenreList();
			if (!is_genre_list_filled) {
				return null;
			}


			JObject movies_json = await movies_json_task;
			if (!JSONHelper.doesJsonContainData(movies_json)) {
				return null;
			}
			var movies = this.getMovieListFromJson(movies_json);
			int movies_max_item_count = movies_json["data"]["total_results"].Value<int>();


			JObject tv_shows_json = await tv_shows_json_task;
			if (!JSONHelper.doesJsonContainData(tv_shows_json)) {
				return null;
			}
			var tv_shows = this.getTvShowListFromJson(tv_shows_json);
			int tv_shows_max_item_count = tv_shows_json["data"]["total_results"].Value<int>();


			List<Show> searched_shows;
			if (movies.Count > tv_shows.Count) {
				searched_shows = movies;
				for (int i_tv_show = 0; i_tv_show < tv_shows.Count; ++i_tv_show) {
					searched_shows.Insert(i_tv_show + 1, tv_shows[i_tv_show]);
				}


			} else {
				searched_shows = tv_shows;
				for (int i_movie = 0; i_movie < movies.Count; ++i_movie) {
					searched_shows.Insert(i_movie + 1, movies[i_movie]);
				}
			}


			int max_item_count;
			if (searched_shows.Count < SHOWS_PER_PAGE) {
				max_item_count = page_number * SHOWS_PER_PAGE;
			} else {
				max_item_count = Math.Max(movies_max_item_count, tv_shows_max_item_count);
			}


			return Tuple.Create(searched_shows, max_item_count);
		}
	}
}