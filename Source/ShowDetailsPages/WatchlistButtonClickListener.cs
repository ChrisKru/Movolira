using Android.Graphics;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Movolira {
	internal class WatchlistButtonClickListener : Object, View.IOnClickListener {
		private readonly MainActivity _main_activity;
		private readonly Button _watchlist_button;
		private readonly Show _show;
		private bool _is_show_watchlisted;




		public WatchlistButtonClickListener(MainActivity main_activity, Button watchlist_button, Show show) {
			_main_activity = main_activity;
			_watchlist_button = watchlist_button;
			_show = show;
			if (main_activity.UserData.isShowInWatchlist(show.Id)) {
				_is_show_watchlisted = true;
				toggleButtonState();
			}
		}




		public void OnClick(View button) {
			if (_is_show_watchlisted) {
				_main_activity.UserData.removeFromWatchlist(_show.Id);
			} else {
				_main_activity.UserData.addToWatchlist(_show);
			}
			_is_show_watchlisted = !_is_show_watchlisted;
			toggleButtonState();
		}




		private void toggleButtonState() {
			if (_is_show_watchlisted) {
				_watchlist_button.Text = _main_activity.GetString(Resource.String.show_details_add_watchlist_button_added);
				_watchlist_button.SetTextColor(new Color(ContextCompat.GetColor(_main_activity, Resource.Color.show_details_button_text_set)));
			} else {
				_watchlist_button.Text = _main_activity.GetString(Resource.String.show_details_add_watchlist_button);
				_watchlist_button.SetTextColor(new Color(ContextCompat.GetColor(_main_activity, Resource.Color.show_details_button_text)));
			}
		}
	}
}