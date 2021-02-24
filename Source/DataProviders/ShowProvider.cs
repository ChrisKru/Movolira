using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;




namespace Movolira.DataProviders {
	public class ShowProvider {
		private GenresProvider _genres_provider;
		private MovieProvider _movie_provider;
		private TvShowProvider _tv_show_provider;




		public ShowProvider(GenresProvider genres_provider, MovieProvider movie_provider, TvShowProvider tv_show_provider) {
			this._genres_provider = genres_provider;
			this._movie_provider = movie_provider;
			this._tv_show_provider = tv_show_provider;
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
			var movies = this._movie_provider.getMovieListFromJson(movies_json);
			int movies_max_item_count = movies_json["data"]["total_results"].Value<int>();


			JObject tv_shows_json = await tv_shows_json_task;
			if (!JSONHelper.doesJsonContainData(tv_shows_json)) {
				return null;
			}
			var tv_shows = this._tv_show_provider.getTvShowListFromJson(tv_shows_json);
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
			if (searched_shows.Count < MainActivity.SHOWS_PER_PAGE) {
				max_item_count = page_number * MainActivity.SHOWS_PER_PAGE;
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
			var movies = this._movie_provider.getMovieListFromJson(movies_json);
			int movies_max_item_count = movies_json["data"]["total_results"].Value<int>();


			JObject tv_shows_json = await tv_shows_json_task;
			if (!JSONHelper.doesJsonContainData(tv_shows_json)) {
				return null;
			}
			var tv_shows = this._tv_show_provider.getTvShowListFromJson(tv_shows_json);
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
			if (searched_shows.Count < MainActivity.SHOWS_PER_PAGE) {
				max_item_count = page_number * MainActivity.SHOWS_PER_PAGE;
			} else {
				max_item_count = Math.Max(movies_max_item_count, tv_shows_max_item_count);
			}


			return Tuple.Create(searched_shows, max_item_count);
		}
	}
}