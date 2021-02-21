using System;
using Android.App;
using Android.Views;
using Android.Widget;




namespace Movolira.Pages.ShowDetailsPages {
	internal class RatingDialog {
		public event EventHandler<int> RatedEvent;


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
			this._layout.FindViewById(Resource.Id.rating_dialog_rating_star_1).Click += this.OnRatingStar1Click;
			this._layout.FindViewById(Resource.Id.rating_dialog_rating_star_2).Click += this.OnRatingStar2Click;
			this._layout.FindViewById(Resource.Id.rating_dialog_rating_star_3).Click += this.OnRatingStar3Click;
			this._layout.FindViewById(Resource.Id.rating_dialog_rating_star_4).Click += this.OnRatingStar4Click;
			this._layout.FindViewById(Resource.Id.rating_dialog_rating_star_5).Click += this.OnRatingStar5Click;


			dialog_builder.SetView(this._layout);
			this._dialog = dialog_builder.Create();
		}




		private void OnCancelButtonClick(object sender, EventArgs args) {
			this._dialog.Hide();
		}




		private void OnRateButtonClick(object sender, EventArgs args) {
			this._main_activity.UserData.addToRatedShows(this._show, this._current_rating);
			RatedEvent.Invoke(this, this._current_rating);
			this._dialog.Hide();
		}




		private void OnRatingStar1Click(object sender, EventArgs args) {
			this.toggleRatingStars(1);
			this._current_rating = 1;
		}




		private void OnRatingStar2Click(object sender, EventArgs args) {
			this.toggleRatingStars(2);
			this._current_rating = 2;
		}




		private void OnRatingStar3Click(object sender, EventArgs args) {
			this.toggleRatingStars(3);
			this._current_rating = 3;
		}




		private void OnRatingStar4Click(object sender, EventArgs args) {
			this.toggleRatingStars(4);
			this._current_rating = 4;
		}




		private void OnRatingStar5Click(object sender, EventArgs args) {
			this.toggleRatingStars(5);
			this._current_rating = 5;
		}




		private void toggleRatingStars(int rating) {
			if (rating >= 2) {
				this._layout.FindViewById<ImageView>(Resource.Id.rating_dialog_rating_star_2)
					.SetImageResource(Resource.Drawable.ic_star_crop_full);
			} else {
				this._layout.FindViewById<ImageView>(Resource.Id.rating_dialog_rating_star_2)
					.SetImageResource(Resource.Drawable.ic_star_crop_empty);
			}


			if (rating >= 3) {
				this._layout.FindViewById<ImageView>(Resource.Id.rating_dialog_rating_star_3)
					.SetImageResource(Resource.Drawable.ic_star_crop_full);
			} else {
				this._layout.FindViewById<ImageView>(Resource.Id.rating_dialog_rating_star_3)
					.SetImageResource(Resource.Drawable.ic_star_crop_empty);
			}


			if (rating >= 4) {
				this._layout.FindViewById<ImageView>(Resource.Id.rating_dialog_rating_star_4)
					.SetImageResource(Resource.Drawable.ic_star_crop_full);
			} else {
				this._layout.FindViewById<ImageView>(Resource.Id.rating_dialog_rating_star_4)
					.SetImageResource(Resource.Drawable.ic_star_crop_empty);
			}


			if (rating >= 5) {
				this._layout.FindViewById<ImageView>(Resource.Id.rating_dialog_rating_star_5)
					.SetImageResource(Resource.Drawable.ic_star_crop_full);
			} else {
				this._layout.FindViewById<ImageView>(Resource.Id.rating_dialog_rating_star_5)
					.SetImageResource(Resource.Drawable.ic_star_crop_empty);
			}
		}




		public void showDialog() {
			this._dialog.Show();
		}
	}
}