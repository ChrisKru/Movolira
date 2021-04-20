using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Movolira.DataProviders;

namespace Movolira.Pages.ShowDetailsPages {
	public class WatchlistButtonClickListener : Object, View.IOnClickListener {
		private MainActivity _main_activity;
		private Button _watchlist_button;
		private Show _show;
		private Color _text_color_not_watchlisted;
		private Color _text_color_watchlisted;
		private bool _is_show_watchlisted;




		public WatchlistButtonClickListener(MainActivity main_activity, Button watchlist_button, Show show) {
			this._main_activity = main_activity;
			this._watchlist_button = watchlist_button;
			this._show = show;


			TypedValue typed_attribute_value = new TypedValue();
			this._main_activity.Theme.ResolveAttribute(Resource.Attribute.textColorC, typed_attribute_value, true);
			this._text_color_watchlisted = new Color(typed_attribute_value.Data);
			this._main_activity.Theme.ResolveAttribute(Resource.Attribute.textColorD, typed_attribute_value, true);
			this._text_color_not_watchlisted = new Color(typed_attribute_value.Data);


			if (UserDataProvider.isShowInWatchlist(show.Id)) {
				this._is_show_watchlisted = true;
				this.toggleButtonState();
			} else {
				this._is_show_watchlisted = false;
			}
		}




		public void OnClick(View button) {
			if (this._is_show_watchlisted) {
				UserDataProvider.removeFromWatchlist(this._show.Id);
			} else {
				UserDataProvider.addToWatchlist(this._show);
			}


			this._is_show_watchlisted = !this._is_show_watchlisted;
			this.toggleButtonState();
		}




		private void toggleButtonState() {
			if (this._is_show_watchlisted) {
				this._watchlist_button.Text = this._main_activity.GetString(Resource.String.show_details_add_watchlist_button_added);
				this._watchlist_button.SetTextColor(this._text_color_watchlisted);


			} else {
				this._watchlist_button.Text = this._main_activity.GetString(Resource.String.show_details_add_watchlist_button);
				this._watchlist_button.SetTextColor(this._text_color_not_watchlisted);
			}
		}
	}
}