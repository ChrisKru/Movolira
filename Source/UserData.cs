﻿using System.Collections.Generic;

namespace Movolira {
	public class UserData {
		public Dictionary<string, Show> Watchlist { get; }




		public UserData() {
			Watchlist = new Dictionary<string, Show>();
		}




		public void addToWatchlist(Show show) {
			Watchlist.Add(show.Id, show);
		}




		public void removeFromWatchlist(string show_id) {
			Watchlist.Remove(show_id);
		}




		public bool isShowInWatchlist(string show_id) {
			return Watchlist.ContainsKey(show_id);
		}
	}
}