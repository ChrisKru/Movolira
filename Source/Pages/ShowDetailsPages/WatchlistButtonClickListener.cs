using Android.Graphics;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Java.Lang;




namespace Movolira.Pages.ShowDetailsPages {
	internal class WatchlistButtonClickListener : Object, View.IOnClickListener {
		private readonly MainActivity _main_activity;
		private readonly Button _watchlist_button;
		private readonly Show _show;
		private bool _is_show_watchlisted;




		public WatchlistButtonClickListener(MainActivity main_activity, Button watchlist_button, Show show) {
			this._main_activity = main_activity;
			this._watchlist_button = watchlist_button;
			this._show = show;


			if (main_activity.UserData.isShowInWatchlist(show.Id)) {
				this._is_show_watchlisted = true;
				this.toggleButtonState();
			}
		}




		public void OnClick(View button) {
			if (this._is_show_watchlisted) {
				this._main_activity.UserData.removeFromWatchlist(this._show.Id);
			} else {
				this._main_activity.UserData.addToWatchlist(this._show);
			}


			this._is_show_watchlisted = !this._is_show_watchlisted;
			this.toggleButtonState();
		}




		private void toggleButtonState() {
			if (this._is_show_watchlisted) {
				this._watchlist_button.Text = this._main_activity.GetString(Resource.String.show_details_add_watchlist_button_added);
				this._watchlist_button.SetTextColor(new Color(ContextCompat.GetColor(this._main_activity,
					Resource.Color.show_details_button_text_set)));
			} else {
				this._watchlist_button.Text = this._main_activity.GetString(Resource.String.show_details_add_watchlist_button);
				this._watchlist_button.SetTextColor(new Color(ContextCompat.GetColor(this._main_activity,
					Resource.Color.show_details_button_text)));
			}
		}
	}
}