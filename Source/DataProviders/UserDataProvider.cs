using System.Collections.Generic;
using System.Linq;
using Realms;




namespace Movolira.DataProviders {
	public static class UserDataProvider {
		public static List<SerializedShow> getWatchlist() {
			Realm realm_db = Realm.GetInstance();
			return realm_db.All<SerializedShow>().ToList();
		}




		public static List<SerializedRatedShow> getFiveStarShows() {
			Realm realm_db = Realm.GetInstance();
			return realm_db.All<SerializedRatedShow>().Where(show => show.Rating == 5).ToList();
		}




		public static List<SerializedRatedShow> getFourStarShows() {
			Realm realm_db = Realm.GetInstance();
			return realm_db.All<SerializedRatedShow>().Where(show => show.Rating == 4).ToList();
		}




		public static List<SerializedRatedShow> getRatedShows() {
			Realm realm_db = Realm.GetInstance();
			return realm_db.All<SerializedRatedShow>().OrderByDescending(show => show.Rating)
				.ThenBy(show => show.Title).ToList();
		}




		public static int getShowRating(string show_id) {
			Realm realm_db = Realm.GetInstance();
			var matching_shows = realm_db.All<SerializedRatedShow>().Where(show => show.Id == show_id);


			if (!matching_shows.Any()) {
				return 0;
			}
			return matching_shows.First().Rating;
		}




		public static HashSet<string> getKnownShowIds() {
			HashSet<string> known_show_ids = new HashSet<string>();
			Realm realm_db = Realm.GetInstance();


			foreach (var show in realm_db.All<SerializedShow>()) {
				known_show_ids.Add(show.Id);
			}
			foreach (var show in realm_db.All<SerializedRatedShow>()) {
				known_show_ids.Add(show.Id);
			}
			foreach (var show in realm_db.All<AlreadyRecommendedShow>()) {
				known_show_ids.Add(show.Id);
			}


			return known_show_ids;
		}




		public static void addToWatchlist(Show show) {
			Realm realm_db = Realm.GetInstance();
			realm_db.Write(() => realm_db.Add(show.serialize()));
		}




		public static void addToRatedShows(Show show, int rating) {
			SerializedShow serialized_show = show.serialize();
			SerializedRatedShow rated_show = new SerializedRatedShow();


			rated_show.Id = serialized_show.Id;
			rated_show.Type = serialized_show.Type;
			rated_show.Title = serialized_show.Title;
			rated_show.Rating = rating;


			Realm realm_db = Realm.GetInstance();
			realm_db.Write(() => realm_db.Add(rated_show));
		}




		public static void addToAlreadyRecommendedShows(Show show) {
			AlreadyRecommendedShow already_recommended = new AlreadyRecommendedShow();
			already_recommended.Id = show.Id;
			Realm realm_db = Realm.GetInstance();
			realm_db.Write(() => realm_db.Add(already_recommended));
		}




		public static void removeFromWatchlist(string show_id) {
			Realm realm_db = Realm.GetInstance();
			var matching_shows = realm_db.All<SerializedShow>().Where(show => show.Id == show_id);
			realm_db.Write(() => realm_db.RemoveRange(matching_shows));
		}




		public static void removeFromRatedShows(string show_id) {
			Realm realm_db = Realm.GetInstance();
			var matching_shows = realm_db.All<SerializedRatedShow>().Where(show => show.Id == show_id);
			realm_db.Write(() => realm_db.RemoveRange(matching_shows));
		}




		public static bool isShowInWatchlist(string show_id) {
			Realm realm_db = Realm.GetInstance();
			var matching_shows = realm_db.All<SerializedShow>().Where(show => show.Id == show_id);
			if (matching_shows.Any()) {
				return true;
			}
			return false;
		}
	}
}