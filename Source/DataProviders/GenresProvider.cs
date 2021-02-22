using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;




namespace Movolira.DataProviders {
	public class GenresProvider {
		private readonly Dictionary<int, string> _genre_list;




		public GenresProvider() {
			this._genre_list = new Dictionary<int, string>();
		}




		public async Task<Dictionary<int, string>> getGenreList() {
			bool is_genre_list_filled = await this.tryFillGenreList();
			if (!is_genre_list_filled) {
				return null;
			}
			return this._genre_list;
		}




		public async Task<bool> tryFillGenreList() {
			if (this._genre_list.Count == 0) {
				await this.fillGenreList();
				if (this._genre_list.Count == 0) {
					return false;
				}
			}
			return true;
		}




		private async Task fillGenreList() {
			Uri tv_show_genres_uri = new Uri("https://api.themoviedb.org/3/genre/tv/list" + "?api_key=" + ApiKeys.TMDB_KEY);
			var tv_show_genres_task = JSONHelper.getJson("tv_genres", tv_show_genres_uri);
			Uri movie_genres_uri = new Uri("https://api.themoviedb.org/3/genre/movie/list" + "?api_key=" + ApiKeys.TMDB_KEY);


			JObject movie_genres_json = await JSONHelper.getJson("movie_genres", movie_genres_uri);
			if (!JSONHelper.doesJsonContainData(movie_genres_json)) {
				return;
			}
			this.addMovieGenresToGenreList(movie_genres_json);


			JObject tv_show_genres_json = await tv_show_genres_task;
			if (!JSONHelper.doesJsonContainData(tv_show_genres_json)) {
				return;
			}
			this.addTvShowGenresToGenreList(tv_show_genres_json);
		}




		private void addMovieGenresToGenreList(JObject movie_genres_json) {
			IList<JToken> movie_genres_jtokens = movie_genres_json["data"]["genres"].Children().ToList();
			foreach (JToken genre_jtoken in movie_genres_jtokens) {
				int id = genre_jtoken["id"].Value<int>();
				string name = genre_jtoken["name"].Value<string>();
				// TMDB API can sometimes send duplicate genres with the same ID
				// The same genres can be returned in both movie and tv show genre lists.
				if (!this._genre_list.ContainsKey(id)) {
					this._genre_list.Add(id, name);
				}
			}
		}




		private void addTvShowGenresToGenreList(JObject tv_show_genres_json) {
			IList<JToken> tv_show_genres_jtokens = tv_show_genres_json["data"]["genres"].Children().ToList();
			foreach (JToken genre_jtoken in tv_show_genres_jtokens) {
				int id = genre_jtoken["id"].Value<int>();
				string name = genre_jtoken["name"].Value<string>();
				// TMDB API can sometimes send duplicate genres with the same ID
				// The same genres can be returned in both movie and tv show genre lists.
				if (!this._genre_list.ContainsKey(id)) {
					this._genre_list.Add(id, name);
				}
			}
		}




		public string getGenreNameForId(int genre_id) {
			return this._genre_list[genre_id];
		}
	}
}