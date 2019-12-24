using Android.Graphics;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Movolira {
	internal class RatingButtonClickListener : Object, View.IOnClickListener {
		private readonly RatingDialog _dialog;
		private readonly MainActivity _main_activity;


		private readonly Button _rating_button;
		private readonly Show _show;
		private bool _is_show_rated;
		private int _rating;




		public RatingButtonClickListener(MainActivity main_activity, Button rating_button, Show show) {
			_main_activity = main_activity;
			_show = show;
			_rating_button = rating_button;


			_dialog = new RatingDialog(_main_activity, show);
			_dialog.RatedEvent += showRated;


			int rating = main_activity.UserData.getShowRating(show.Id);
			if (rating != 0) {
				_is_show_rated = true;
				_rating = rating;
				toggleButtonState();
			}
		}




		private void showRated(object sender, int rating) {
			_is_show_rated = true;
			_rating = rating;
			toggleButtonState();
		}




		public void OnClick(View view) {
			if (_is_show_rated) {
				_is_show_rated = false;
				_main_activity.UserData.removeFromRatedShows(_show.Id);
				toggleButtonState();
			} else {
				_dialog.showDialog();
			}
		}




		private void toggleButtonState() {
			if (_is_show_rated) {
				_rating_button.Text = "Rated " + _rating + "/5";
				_rating_button.SetTextColor(new Color(ContextCompat.GetColor(_main_activity, Resource.Color.show_details_button_text_set)));
			} else {
				_rating_button.Text = _main_activity.GetString(Resource.String.show_details_add_rating_button);
				_rating_button.SetTextColor(new Color(ContextCompat.GetColor(_main_activity, Resource.Color.show_details_button_text)));
			}
		}
	}
}