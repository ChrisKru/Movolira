using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;




namespace Movolira.DataProviders {
	public class GenresProvider {
		// Maps genre names to genre ids.
		public SortedDictionary<string, List<int>> _genre_id_dict;


		// Maps genre ids to genre names.
		// This is generated based on TMDB's API.
		// It's not statically mapped, because the genres might change with time.
		//
		// A single id might point to multiple strings, because some genres are represented multiple times in a combined form.
		// For example "Sci-Fi & Action" is split to "Sci-Fi" and "Action".
		private Dictionary<int, List<string>> _genre_dict;




		public GenresProvider() {
			this._genre_dict = new Dictionary<int, List<string>>();
			this._genre_id_dict = new SortedDictionary<string, List<int>>();
		}




		public List<int> getGenreIdsForName(string genre_name) {
			return this._genre_id_dict[genre_name];
		}




		public List<string> getGenreNamesForId(int genre_id) {
			return this._genre_dict[genre_id];
		}




		public SortedDictionary<string, List<int>> getGenreIdDict() {
			return this._genre_id_dict;
		}




		public async Task<bool> tryFillGenreDict() {
			if (this._genre_dict.Count == 0) {
				await this.fillGenreDict();
				if (this._genre_dict.Count == 0) {
					return false;
				}
			}
			return true;
		}




		private async Task fillGenreDict() {
			Uri tv_show_genres_uri = new Uri("https://api.themoviedb.org/3/genre/tv/list" + "?api_key=" + ApiKeys.TMDB_KEY);
			var tv_show_genres_task = JSONHelper.getJson("tv_genres", tv_show_genres_uri);
			Uri movie_genres_uri = new Uri("https://api.themoviedb.org/3/genre/movie/list" + "?api_key=" + ApiKeys.TMDB_KEY);


			JObject movie_genres_json = await JSONHelper.getJson("movie_genres", movie_genres_uri);
			if (!JSONHelper.doesJsonContainData(movie_genres_json)) {
				return;
			}
			this.addMovieGenresToGenreDict(movie_genres_json);


			JObject tv_show_genres_json = await tv_show_genres_task;
			if (!JSONHelper.doesJsonContainData(tv_show_genres_json)) {
				return;
			}
			this.addTvShowGenresToGenreDict(tv_show_genres_json);


			this.fillGenreIdDict();
		}




		private void addMovieGenresToGenreDict(JObject movie_genres_json) {
			IList<JToken> movie_genres_jtokens = movie_genres_json["data"]["genres"].Children().ToList();
			foreach (JToken genre_jtoken in movie_genres_jtokens) {
				int id = genre_jtoken["id"].Value<int>();
				string name = genre_jtoken["name"].Value<string>();


				// TMDB returns 'Sci-Fi' and 'Science Fiction' as different genres.
				// The longer version is replaced for consistency
				if (name == "Science Fiction") {
					name = "Sci-Fi";
				}


				// TMDB API can sometimes send duplicate genres with the same ID
				// The same genres can be returned in both movie and tv show genre lists.
				if (!this._genre_dict.ContainsKey(id)) {
					var names = new List<string>();
					names.Add(name);
					this._genre_dict.Add(id, names);
				}
			}
		}




		private void addTvShowGenresToGenreDict(JObject tv_show_genres_json) {
			IList<JToken> tv_show_genres_jtokens = tv_show_genres_json["data"]["genres"].Children().ToList();
			foreach (JToken genre_jtoken in tv_show_genres_jtokens) {
				int id = genre_jtoken["id"].Value<int>();
				var names = new List<string>();
				string name = genre_jtoken["name"].Value<string>();


				// TMDB returns 'Sci-Fi' and 'Science Fiction' as different genres.
				// The longer version is replaced for consistency
				if (name == "Science Fiction") {
					name = "Sci-Fi";
				}


				// Splits combined genres (those only appeared in tv show requests at the time of writing this code).
				// A single id might point to multiple strings, because some genres are represented multiple times in a combined form.
				// For example "Drama & Action" is split to "Drama" and "Action".
				if (name.Contains("&")) {
					var split_names = name.Split(new[] { '&', ' ' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string split_name in split_names) {
						names.Add(split_name);
					}
				} else {
					names.Add(name);
				}


				// TMDB API can sometimes send duplicate genres with the same ID
				// The same genres can be returned in both movie and tv show genre lists.
				if (!this._genre_dict.ContainsKey(id)) {
					this._genre_dict.Add(id, names);
				}
			}
		}




		private void fillGenreIdDict() {
			foreach (var genre_dict_entry in this._genre_dict) {
				foreach (string genre_name in genre_dict_entry.Value) {


					var genre_ids = new List<int>();
					if (this._genre_id_dict.ContainsKey(genre_name)) {
						genre_ids.AddRange(this._genre_id_dict[genre_name]);
					}


					genre_ids.Add(genre_dict_entry.Key);
					this._genre_id_dict[genre_name] = genre_ids;
				}
			}
		}
	}
}