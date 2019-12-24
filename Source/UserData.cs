using System.Collections.Generic;
using System.Linq;
using Realms;

namespace Movolira {
	public class UserData {
		private readonly Realm realm_db;




		public UserData() {
			realm_db = Realm.GetInstance();
		}




		public void addToWatchlist(Show show) {
			realm_db.Write(() => realm_db.Add(show.serialize()));
		}




		public void addToRatedShows(Show show, int rating) {
			ShowSerialized serialized_show = show.serialize();
			RatedShowSerialized rated_show = new RatedShowSerialized();


			rated_show.Id = serialized_show.Id;
			rated_show.Type = serialized_show.Type;
			rated_show.Title = serialized_show.Title;
			rated_show.Rating = rating;


			realm_db.Write(() => realm_db.Add(rated_show));
		}




		public void removeFromWatchlist(string show_id) {
			var matching_shows = realm_db.All<ShowSerialized>().Where(show => show.Id == show_id);


			realm_db.Write(() => realm_db.RemoveRange(matching_shows));
		}




		public void removeFromRatedShows(string show_id) {
			var matching_shows = realm_db.All<RatedShowSerialized>().Where(show => show.Id == show_id);


			realm_db.Write(() => realm_db.RemoveRange(matching_shows));
		}




		public List<ShowSerialized> getWatchlist() {
			return realm_db.All<ShowSerialized>().ToList();
		}




		public List<RatedShowSerialized> getRatedShows() {
			return realm_db.All<RatedShowSerialized>().ToList();
		}




		public bool isShowInWatchlist(string show_id) {
			var matching_shows = realm_db.All<ShowSerialized>().Where(show => show.Id == show_id);


			if (matching_shows.Any()) {
				return true;
			}
			return false;
		}




		public int getShowRating(string show_id) {
			var matching_shows = realm_db.All<RatedShowSerialized>().Where(show => show.Id == show_id);


			if (!matching_shows.Any()) {
				return 0;
			}
			return matching_shows.First().Rating;
		}
	}
}