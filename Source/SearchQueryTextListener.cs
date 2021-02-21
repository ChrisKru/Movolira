﻿using Android.Support.V7.Widget;
using Android.Views;
using Java.Lang;




namespace Movolira {
	public class SearchQueryTextListener : Object, SearchView.IOnQueryTextListener {
		private readonly MainActivity _main_activity;
		private readonly IMenuItem _search_item;




		public SearchQueryTextListener(MainActivity main_activity, IMenuItem search_item) {
			this._main_activity = main_activity;
			this._search_item = search_item;
		}




		public bool OnQueryTextChange(string newText) {
			return false;
		}




		public bool OnQueryTextSubmit(string query) {
			this._main_activity.submitSearch(query);
			this._search_item.CollapseActionView();
			return true;
		}
	}
}