using Android.Graphics;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Java.Lang;




namespace Movolira.Pages.ShowDetailsPages {
	internal class RatingButtonClickListener : Object, View.IOnClickListener {
		private readonly RatingDialog _dialog;
		private readonly MainActivity _main_activity;
		private readonly Button _rating_button;
		private readonly Show _show;
		private bool _is_show_rated;
		private int _rating;




		public RatingButtonClickListener(MainActivity main_activity, Button rating_button, Show show) {
			this._main_activity = main_activity;
			this._show = show;
			this._rating_button = rating_button;
			this._dialog = new RatingDialog(this._main_activity, show);
			this._dialog.RatedEvent += this.showRated;


			int rating = main_activity.UserData.getShowRating(show.Id);
			if (rating != 0) {
				this._is_show_rated = true;
				this._rating = rating;
				this.toggleButtonState();
			}
		}




		private void showRated(object sender, int rating) {
			this._is_show_rated = true;
			this._rating = rating;
			this.toggleButtonState();
		}




		public void OnClick(View view) {
			if (this._is_show_rated) {
				this._is_show_rated = false;
				this._main_activity.UserData.removeFromRatedShows(this._show.Id);
				this.toggleButtonState();


			} else {
				this._dialog.showDialog();
			}
		}




		private void toggleButtonState() {
			if (this._is_show_rated) {
				this._rating_button.Text = "Rated " + this._rating + "/5";
				this._rating_button.SetTextColor(new Color(
					ContextCompat.GetColor(this._main_activity, Resource.Color.show_details_button_text_set)));
			} else {
				this._rating_button.Text = this._main_activity.GetString(Resource.String.show_details_add_rating_button);
				this._rating_button.SetTextColor(new Color(
					ContextCompat.GetColor(this._main_activity, Resource.Color.show_details_button_text)));
			}
		}
	}
}