using System;
using Android.App;
using Android.Views;
using Android.Widget;




namespace Movolira.Pages.ShowDetailsPages {
	public class RatingDialog {
		public event EventHandler<int> OnShowRatedEvent;


		private readonly int[] RATING_STAR_VIEW_RESOURCE_IDS = {
			Resource.Id.rating_dialog_rating_star_1,
			Resource.Id.rating_dialog_rating_star_2,
			Resource.Id.rating_dialog_rating_star_3,
			Resource.Id.rating_dialog_rating_star_4,
			Resource.Id.rating_dialog_rating_star_5,
		};


		private AlertDialog _dialog;
		private View _layout;
		private readonly MainActivity _main_activity;
		private readonly Show _show;
		private int _current_rating = 3;




		public RatingDialog(MainActivity main_activity, Show show) {
			this._main_activity = main_activity;
			this._show = show;
			this.buildDialog();
		}




		private void buildDialog() {
			AlertDialog.Builder dialog_builder = new AlertDialog.Builder(this._main_activity);
			this._layout = this._main_activity.LayoutInflater.Inflate(Resource.Layout.rating_dialog, null);
			this._layout.FindViewById(Resource.Id.rating_dialog_cancel_button).Click += this.OnCancelButtonClick;
			this._layout.FindViewById(Resource.Id.rating_dialog_rate_button).Click += this.OnRateButtonClick;


			this._layout.FindViewById(Resource.Id.rating_dialog_rating_star_1).Click
				+= (object s, EventArgs a) => this.OnRatingStarClick(1);
			this._layout.FindViewById(Resource.Id.rating_dialog_rating_star_2).Click
				+= (object s, EventArgs a) => this.OnRatingStarClick(2);
			this._layout.FindViewById(Resource.Id.rating_dialog_rating_star_3).Click
				+= (object s, EventArgs a) => this.OnRatingStarClick(3);
			this._layout.FindViewById(Resource.Id.rating_dialog_rating_star_4).Click
				+= (object s, EventArgs a) => this.OnRatingStarClick(4);
			this._layout.FindViewById(Resource.Id.rating_dialog_rating_star_5).Click
				+= (object s, EventArgs a) => this.OnRatingStarClick(5);


			dialog_builder.SetView(this._layout);
			this._dialog = dialog_builder.Create();
		}




		private void OnCancelButtonClick(object sender, EventArgs args) {
			this._dialog.Hide();
		}




		private void OnRateButtonClick(object sender, EventArgs args) {
			this._main_activity.UserData.addToRatedShows(this._show, this._current_rating);
			OnShowRatedEvent.Invoke(this, this._current_rating);
			this._dialog.Hide();
		}




		private void OnRatingStarClick(int i_rating_star) {
			this.toggleRatingStars(i_rating_star);
			this._current_rating = i_rating_star;
		}




		private void toggleRatingStars(int new_rating) {
			for (int i_rating = 1; i_rating < 5; ++i_rating) {
				if (new_rating >= i_rating + 1) {
					this._layout.FindViewById<ImageView>(this.RATING_STAR_VIEW_RESOURCE_IDS[i_rating])
						.SetImageResource(Resource.Drawable.ic_star_crop_full);
				} else {
					this._layout.FindViewById<ImageView>(this.RATING_STAR_VIEW_RESOURCE_IDS[i_rating])
						.SetImageResource(Resource.Drawable.ic_star_crop_empty);
				}
			}
		}




		public void showDialog() {
			this._dialog.Show();
		}
	}
}