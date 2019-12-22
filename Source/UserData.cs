using System.Collections.Generic;
using System.Linq;
using Realms;

namespace Movolira {
	public class UserData {
		private Realm realm_db;




		public UserData() {
			realm_db = Realm.GetInstance();
		}




		public void addToWatchlist(Show show) {
			realm_db.Write(() => realm_db.Add(show.serialize()));
		}




		public void removeFromWatchlist(string show_id) {
			var matching_shows = realm_db.All<ShowSerialized>()
				.Where(show => show.Id == show_id);


			realm_db.Write(() => realm_db.RemoveRange(matching_shows));
		}




		public List<ShowSerialized> getWatchlist() {
			return realm_db.All<ShowSerialized>().ToList();
		}




		public bool isShowInWatchlist(string show_id) {
			var matching_shows = realm_db.All<ShowSerialized>()
				.Where(show => show.Id == show_id);


			if (matching_shows.Any()) {
				return true;
			}
			return false;
		}
	}
}