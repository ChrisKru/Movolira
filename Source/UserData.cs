using System.Collections.Generic;
using System.Linq;
using Realms;




namespace Movolira {
	public class UserData {
		public List<ShowSerialized> getWatchlist() {
			Realm realm_db = Realm.GetInstance();
			return realm_db.All<ShowSerialized>().ToList();
		}




		public List<RatedShowSerialized> getFiveStarShows() {
			Realm realm_db = Realm.GetInstance();
			return realm_db.All<RatedShowSerialized>().Where(show => show.Rating == 5).ToList();
		}




		public List<RatedShowSerialized> getFourStarShows() {
			Realm realm_db = Realm.GetInstance();
			return realm_db.All<RatedShowSerialized>().Where(show => show.Rating == 4).ToList();
		}




		public List<RatedShowSerialized> getRatedShows() {
			Realm realm_db = Realm.GetInstance();
			return realm_db.All<RatedShowSerialized>().OrderByDescending(show => show.Rating)
				.ThenBy(show => show.Title).ToList();
		}




		public int getShowRating(string show_id) {
			Realm realm_db = Realm.GetInstance();
			var matching_shows = realm_db.All<RatedShowSerialized>().Where(show => show.Id == show_id);


			if (!matching_shows.Any()) {
				return 0;
			}
			return matching_shows.First().Rating;
		}




		public HashSet<string> getKnownShowIds() {
			HashSet<string> known_show_ids = new HashSet<string>();
			Realm realm_db = Realm.GetInstance();


			foreach (var show in realm_db.All<ShowSerialized>()) {
				known_show_ids.Add(show.Id);
			}
			foreach (var show in realm_db.All<RatedShowSerialized>()) {
				known_show_ids.Add(show.Id);
			}
			foreach (var show in realm_db.All<AlreadyRecommendedShow>()) {
				known_show_ids.Add(show.Id);
			}


			return known_show_ids;
		}




		public void addToWatchlist(Show show) {
			Realm realm_db = Realm.GetInstance();
			realm_db.Write(() => realm_db.Add(show.serialize()));
		}




		public void addToRatedShows(Show show, int rating) {
			ShowSerialized serialized_show = show.serialize();
			RatedShowSerialized rated_show = new RatedShowSerialized();


			rated_show.Id = serialized_show.Id;
			rated_show.Type = serialized_show.Type;
			rated_show.Title = serialized_show.Title;
			rated_show.Rating = rating;


			Realm realm_db = Realm.GetInstance();
			realm_db.Write(() => realm_db.Add(rated_show));
		}




		public void addToAlreadyRecommendedShows(Show show) {
			AlreadyRecommendedShow already_recommended = new AlreadyRecommendedShow();
			already_recommended.Id = show.Id;
			Realm realm_db = Realm.GetInstance();
			realm_db.Write(() => realm_db.Add(already_recommended));
		}




		public void removeFromWatchlist(string show_id) {
			Realm realm_db = Realm.GetInstance();
			var matching_shows = realm_db.All<ShowSerialized>().Where(show => show.Id == show_id);
			realm_db.Write(() => realm_db.RemoveRange(matching_shows));
		}




		public void removeFromRatedShows(string show_id) {
			Realm realm_db = Realm.GetInstance();
			var matching_shows = realm_db.All<RatedShowSerialized>().Where(show => show.Id == show_id);
			realm_db.Write(() => realm_db.RemoveRange(matching_shows));
		}




		public bool isShowInWatchlist(string show_id) {
			Realm realm_db = Realm.GetInstance();
			var matching_shows = realm_db.All<ShowSerialized>().Where(show => show.Id == show_id);


			if (matching_shows.Any()) {
				return true;
			}
			return false;
		}
	}
}