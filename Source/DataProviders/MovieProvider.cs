using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;




namespace Movolira.DataProviders {
	public class MovieProvider {
		private GenresProvider _genres_provider;




		public MovieProvider(GenresProvider genres_provider) {
			this._genres_provider = genres_provider;
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




		public List<Show> getMovieListFromJson(JObject movies_json) {
			var movies = new List<Show>();
			IList<JToken> movies_jtokens = movies_json["data"]["results"].Children().ToList();


			foreach (JToken movie_jtoken in movies_jtokens) {
				var genre_ids = JSONHelper.getJTokenValueList<int>(movie_jtoken, "genre_ids");
				var genres = new List<string>();
				foreach (int genre_id in genre_ids) {
					genres.Add(this._genres_provider.getGenreNameForId(genre_id));
				}


				// TMDB returns shows with no genres, even if genre inclusion/exclusion is specified in the request.
				// Which is why they'd normally show up on every discover/search regardless of options.
				// These show entries are ignored by data providers, for consistency. 
				if (genres.Count == 0) {
					continue;
				}


				string id = JSONHelper.getJTokenValue<string>(movie_jtoken, "id");
				string title = JSONHelper.getJTokenValue<string>(movie_jtoken, "title");
				string release_date = JSONHelper.getJTokenValue<string>(movie_jtoken, "release_date");
				double rating = JSONHelper.getJTokenValue<double>(movie_jtoken, "vote_average");
				int vote_count = JSONHelper.getJTokenValue<int>(movie_jtoken, "vote_count");
				string overview = JSONHelper.getJTokenValue<string>(movie_jtoken, "overview");
				string poster_url = "http://image.tmdb.org/t/p/w500/"
					+ JSONHelper.getJTokenValue<string>(movie_jtoken, "poster_path");
				string backdrop_url = "http://image.tmdb.org/t/p/w780/"
					+ JSONHelper.getJTokenValue<string>(movie_jtoken, "backdrop_path");



				Movie movie = new Movie(ShowType.Movie, id, title, genres.ToArray(),
					poster_url, backdrop_url, release_date, rating, vote_count, overview);
				movies.Add(movie);
			}


			return movies;
		}




		public async Task<bool> getMovieDetails(Movie movie) {
			Uri details_uri = new Uri("https://api.themoviedb.org/3/movie/" + movie.Id + "?api_key=" + ApiKeys.TMDB_KEY);
			var details_task = JSONHelper.getJson("movie;" + movie.Id, details_uri);
			Uri certification_uri = new Uri("https://api.themoviedb.org/3/movie/" + movie.Id
				+ "/release_dates?api_key=" + ApiKeys.TMDB_KEY);
			var certification_task = JSONHelper.getJson("movie_release_dates;" + movie.Id, certification_uri);


			JObject details_json = await details_task;
			if (!JSONHelper.doesJsonContainData(details_json)) {
				return false;
			}
			if (!movie.AreShowMainDetailsFetched) {
				if (!this.fillMainMovieDetails(movie, details_json)) {
					return false;
				}
			}


			// Extra data, which isn't fetched on TMDB page/list requests.
			movie.Runtime = JSONHelper.getJTokenValue<int>(details_json["data"], "runtime");
			JObject certification_json = await certification_task;
			if (!JSONHelper.doesJsonContainData(certification_json)) {
				return true;
			}
			this.fillMovieCertification(movie, certification_json);


			return true;
		}




		// "Main Details" refers to the fields that are bundled together with page listing requests of TMDB API.
		// The field is used to omit reinitializing those fields, when fetching other show details.
		private bool fillMainMovieDetails(Movie movie, JObject details_json) {
			var movie_genre_jtokens = JSONHelper.getJTokenList(details_json["data"], "genres");
			var movie_genres = new List<string>();
			foreach (JToken genre_jtoken in movie_genre_jtokens) {
				int genre_id = JSONHelper.getJTokenValue<int>(genre_jtoken, "id");
				movie_genres.Add(this._genres_provider.getGenreNameForId(genre_id));
			}


			// TMDB returns shows with no genres even if genre inclusion/exclusion is specified in the request.
			// Which is why they'd normally show up on every discover/search regardless of options.
			// These show entries are ignored by data providers, for consistency.
			if (movie_genres.Count == 0) {
				return false;
			}
			movie.Genres = movie_genres.ToArray();


			movie.ReleaseDate = JSONHelper.getJTokenValue<string>(details_json["data"], "release_date");
			movie.Rating = JSONHelper.getJTokenValue<double>(details_json["data"], "vote_average");
			movie.Votes = JSONHelper.getJTokenValue<int>(details_json["data"], "vote_count");
			movie.Overview = JSONHelper.getJTokenValue<string>(details_json["data"], "overview");
			movie.PosterUrl = "http://image.tmdb.org/t/p/w500/"
				+ JSONHelper.getJTokenValue<string>(details_json["data"], "poster_path");
			movie.BackdropUrl = "http://image.tmdb.org/t/p/w780/"
				+ JSONHelper.getJTokenValue<string>(details_json["data"], "backdrop_path");


			movie.AreShowMainDetailsFetched = true;
			return true;
		}




		private void fillMovieCertification(Movie movie, JObject certification_json) {
			var certification_jtokens = JSONHelper.getJTokenList(certification_json["data"], "results");
			foreach (JToken certification_jtoken in certification_jtokens) {
				string certification_iso_type = JSONHelper.getJTokenValue<string>(certification_jtoken, "iso_3166_1");
				// The method is only looking for 'DE' certification, due to it being most understandable internationally
				// The DE certification consists of a single number,
				// which represents the minimum allowed age (12, 16, 18, etc.)
				if (certification_iso_type == "DE") {
					JToken certification_details = JSONHelper.getJTokenValue<IList<JToken>>(certification_jtoken, "release_dates")[0];
					string certification = JSONHelper.getJTokenValue<string>(certification_details, "certification");
					if (certification != "") {
						movie.Certification = certification + "+";
					}
					return;
				}
			}
		}
	}
}