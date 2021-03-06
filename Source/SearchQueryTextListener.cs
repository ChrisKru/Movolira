using Android.Views;
using AndroidX.AppCompat.Widget;
using Java.Lang;




namespace Movolira {
	public class SearchQueryTextListener : Object, SearchView.IOnQueryTextListener {
		private MainActivity _main_activity;
		private IMenuItem _search_item;




		public SearchQueryTextListener(MainActivity main_activity, IMenuItem search_item) {
			this._main_activity = main_activity;
			this._search_item = search_item;
		}




		// This is only here, because the ''IOnQueryTextListener' interface,
		// forces the implementation of it.
		public bool OnQueryTextChange(string newText) {
			return false; // perform default action
		}




		public bool OnQueryTextSubmit(string query) {
			this._main_activity.submitSearch(query);
			this._search_item.CollapseActionView();
			return true;
		}
	}
}