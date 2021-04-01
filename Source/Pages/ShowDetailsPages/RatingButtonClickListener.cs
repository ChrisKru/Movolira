using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using Java.Lang;




namespace Movolira.Pages.ShowDetailsPages {
	public class RatingButtonClickListener : Object, View.IOnClickListener {
		private readonly RatingDialog _rating_dialog;
		private readonly MainActivity _main_activity;
		private readonly Button _rating_button;
		private readonly Show _show;
		private bool _is_show_rated;
		private int _rating;




		public RatingButtonClickListener(MainActivity main_activity, Button rating_button, Show show) {
			this._main_activity = main_activity;
			this._show = show;
			this._rating_button = rating_button;
			this._rating_dialog = new RatingDialog(this._main_activity, show);
			this._rating_dialog.OnShowRatedEvent += this.OnShowRatedEvent;


			int rating = UserDataProvider.getShowRating(show.Id);
			if (rating != 0) {
				this._is_show_rated = true;
				this._rating = rating;
				this.toggleRatingButtonState();


			} else {
				this._is_show_rated = false;
			}
		}




		private void OnShowRatedEvent(object sender, int rating) {
			this._is_show_rated = true;
			this._rating = rating;
			this.toggleRatingButtonState();
		}




		public void OnClick(View view) {
			if (this._is_show_rated) {
				this._is_show_rated = false;
				UserDataProvider.removeFromRatedShows(this._show.Id);
				this.toggleRatingButtonState();


			} else {
				this._rating_dialog.showDialog();
			}
		}




		private void toggleRatingButtonState() {
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