using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Movolira.DataProviders;

namespace Movolira.Pages.ShowDetailsPages {
	public class RatingButtonClickListener : Object, View.IOnClickListener {
		private MainActivity _main_activity;
		private RatingDialog _rating_dialog;
		private Button _rating_button;
		private Show _show;
		private Color _text_color_unrated;
		private Color _text_color_rated;
		private bool _is_show_rated;
		private int _rating;




		public RatingButtonClickListener(MainActivity main_activity, Button rating_button, Show show) {
			this._main_activity = main_activity;
			this._show = show;
			this._rating_button = rating_button;
			this._rating_dialog = new RatingDialog(this._main_activity, show);
			this._rating_dialog.OnShowRatedEvent += this.OnShowRatedEvent;


			TypedValue typed_attribute_value = new TypedValue();
			this._main_activity.Theme.ResolveAttribute(Resource.Attribute.textColorC, typed_attribute_value, true);
			this._text_color_rated = new Color(typed_attribute_value.Data);
			this._main_activity.Theme.ResolveAttribute(Resource.Attribute.textColorD, typed_attribute_value, true);
			this._text_color_unrated = new Color(typed_attribute_value.Data);


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
				this._rating_button.SetTextColor(this._text_color_rated);


			} else {
				this._rating_button.Text = this._main_activity.GetString(Resource.String.show_details_add_rating_button);
				this._rating_button.SetTextColor(this._text_color_unrated);
			}
		}
	}
}