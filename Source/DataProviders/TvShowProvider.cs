using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;




namespace Movolira.DataProviders {
	public class TvShowProvider {
		private GenresProvider _genres_provider;




		public TvShowProvider(GenresProvider genres_provider) {
			this._genres_provider = genres_provider;
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




		public List<Show> getTvShowListFromJson(JObject tv_shows_json) {
			var tv_shows = new List<Show>();
			IList<JToken> tv_shows_jtokens = tv_shows_json["data"]["results"].Children().ToList();


			foreach (JToken tv_show_jtoken in tv_shows_jtokens) {
				var genre_ids = JSONHelper.getJTokenValueList<int>(tv_show_jtoken, "genre_ids");
				var genres = this.getTvShowGenres(genre_ids);
				// TMDB returns shows with no genres, even if genre inclusion/exclusion is specified in the request.
				// Which is why they'd normally show up on every discover/search regardless of options.
				// These show entries are ignored by data providers, for consistency. 
				if (genres.Count == 0) {
					continue;
				}


				string id = JSONHelper.getJTokenValue<string>(tv_show_jtoken, "id");
				string title = JSONHelper.getJTokenValue<string>(tv_show_jtoken, "name");
				string release_date = JSONHelper.getJTokenValue<string>(tv_show_jtoken, "first_air_date");
				double rating = JSONHelper.getJTokenValue<double>(tv_show_jtoken, "vote_average");
				int vote_count = JSONHelper.getJTokenValue<int>(tv_show_jtoken, "vote_count");
				string overview = JSONHelper.getJTokenValue<string>(tv_show_jtoken, "overview");
				string poster_url = "http://image.tmdb.org/t/p/w500/"
					+ JSONHelper.getJTokenValue<string>(tv_show_jtoken, "poster_path");
				string backdrop_url = "http://image.tmdb.org/t/p/w780/"
					+ JSONHelper.getJTokenValue<string>(tv_show_jtoken, "backdrop_path");


				TvShow tv_show = new TvShow(ShowType.TvShow, id, title, genres.ToArray(), poster_url,
					backdrop_url, release_date, rating, vote_count, overview);
				tv_shows.Add(tv_show);
			}


			return tv_shows;
		}




		private List<string> getTvShowGenres(List<int> genre_ids) {
			var genres = new List<string>();
			foreach (int genre_id in genre_ids) {
				string genre = this._genres_provider.getGenreNameForId(genre_id);
				if (!genre.Contains("&")) {
					genres.Add(genre);


					// Tv shows returned by TMDB API might contain a combination of multiple genres within a single entry.
					// These entries are split and taken as multiple genres.
				} else {
					var split_genres = genre.Split(new[] { '&', ' ' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string split_genre in split_genres) {
						genres.Add(split_genre);
					}
				}
			}


			return genres;
		}




		public async Task<bool> getTvShowDetails(TvShow tv_show) {
			Uri details_uri = new Uri("https://api.themoviedb.org/3/tv/" + tv_show.Id + "?api_key=" + ApiKeys.TMDB_KEY);
			var details_task = JSONHelper.getJson("tv_show;" + tv_show.Id, details_uri);
			Uri certification_uri = new Uri("https://api.themoviedb.org/3/tv/"
				+ tv_show.Id + "/content_ratings?api_key=" + ApiKeys.TMDB_KEY);
			var certification_task = JSONHelper.getJson("tv_show_content_ratings;" + tv_show.Id, certification_uri);


			JObject details_json = await details_task;
			if (!JSONHelper.doesJsonContainData(details_json)) {
				return false;
			}
			if (!tv_show.AreShowMainDetailsFetched) {
				if (!this.fillMainTvShowDetails(tv_show, details_json)) {
					return false;
				}
			}


			this.fillTvShowRuntime(tv_show, details_json);
			JObject certification_json = await certification_task;
			if (!JSONHelper.doesJsonContainData(certification_json)) {
				return true;
			}
			this.fillTvShowCertification(tv_show, certification_json);


			return true;
		}




		// "Main Details" refers to the fields that are bundled together with page listing requests of TMDB API.
		// The field is used to omit reinitializing those fields, when fetching other show details.
		private bool fillMainTvShowDetails(TvShow tv_show, JObject details_json) {
			var tv_show_genre_jtokens = JSONHelper.getJTokenList(details_json["data"], "genres");
			var tv_show_genres = new List<string>();
			foreach (JToken genre_jtoken in tv_show_genre_jtokens) {
				int genre_id = JSONHelper.getJTokenValue<int>(genre_jtoken, "id");
				tv_show_genres.Add(this._genres_provider.getGenreNameForId(genre_id));
			}



			// TMDB returns shows with no genres even if genre inclusion/exclusion is specified in the request.
			// Which is why they'd normally show up on every discover/search regardless of options.
			// These show entries are ignored by data providers, for consistency.
			if (tv_show_genres.Count == 0) {
				return false;
			}
			tv_show.Genres = tv_show_genres.ToArray();


			tv_show.AirDate = JSONHelper.getJTokenValue<string>(details_json["data"], "first_air_date");
			tv_show.Rating = JSONHelper.getJTokenValue<double>(details_json["data"], "vote_average");
			tv_show.Votes = JSONHelper.getJTokenValue<int>(details_json["data"], "vote_count");
			tv_show.Overview = JSONHelper.getJTokenValue<string>(details_json["data"], "overview");
			tv_show.PosterUrl = "http://image.tmdb.org/t/p/w500/"
				+ JSONHelper.getJTokenValue<string>(details_json["data"], "poster_path");
			tv_show.BackdropUrl = "http://image.tmdb.org/t/p/w780/"
				+ JSONHelper.getJTokenValue<string>(details_json["data"], "backdrop_path");


			tv_show.AreShowMainDetailsFetched = true;
			return true;
		}




		private void fillTvShowRuntime(TvShow tv_show, JObject details_json) {
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
		}




		private void fillTvShowCertification(TvShow tv_show, JObject certification_json) {
			var certification_jtokens = JSONHelper.getJTokenList(certification_json["data"], "results");
			foreach (JToken certification_jtoken in certification_jtokens) {
				string certification_iso_type = JSONHelper.getJTokenValue<string>(certification_jtoken, "iso_3166_1");
				// The method is only looking for 'DE' certification, due to it being most understandable internationally
				// The DE certification consists of a single number,
				// which represents the minimum allowed age (12, 16, 18, etc.)
				if (certification_iso_type == "DE") {
					string certification = JSONHelper.getJTokenValue<string>(certification_jtoken, "rating");
					if (certification != "") {
						tv_show.Certification = certification + "+";
					}
					return;
				}
			}
		}
	}
}