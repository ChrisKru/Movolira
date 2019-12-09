using System;
using Android.App;
using Android.Views;
using Android.Widget;

namespace Movolira {
	internal class RatingDialog {
		private AlertDialog _dialog;
		private View _layout;
		private MainActivity _main_activity;




		public RatingDialog(MainActivity main_activity) {
			_main_activity = main_activity;
			buildDialog();
		}




		private void buildDialog() {
			AlertDialog.Builder dialog_builder = new AlertDialog.Builder(_main_activity);
			_layout = _main_activity.LayoutInflater.Inflate(Resource.Layout.rating_dialog, null);


			_layout.FindViewById(Resource.Id.rating_dialog_cancel_button).Click += OnCancelButtonClick;
			_layout.FindViewById(Resource.Id.rating_dialog_rate_button).Click += OnRateButtonClick;


			_layout.FindViewById(Resource.Id.rating_dialog_rating_star_1).Click += OnRatingStar1Click;
			_layout.FindViewById(Resource.Id.rating_dialog_rating_star_2).Click += OnRatingStar2Click;
			_layout.FindViewById(Resource.Id.rating_dialog_rating_star_3).Click += OnRatingStar3Click;
			_layout.FindViewById(Resource.Id.rating_dialog_rating_star_4).Click += OnRatingStar4Click;
			_layout.FindViewById(Resource.Id.rating_dialog_rating_star_5).Click += OnRatingStar5Click;


			dialog_builder.SetView(_layout);
			_dialog = dialog_builder.Create();
		}




		private void OnCancelButtonClick(object sender, EventArgs args) {
			_dialog.Hide();
		}




		private void OnRateButtonClick(object sender, EventArgs args) {
			_dialog.Hide();
		}




		private void OnRatingStar1Click(object sender, EventArgs args) {
			toggleRatingStars(1);
		}




		private void OnRatingStar2Click(object sender, EventArgs args) {
			toggleRatingStars(2);
		}




		private void OnRatingStar3Click(object sender, EventArgs args) {
			toggleRatingStars(3);
		}




		private void OnRatingStar4Click(object sender, EventArgs args) {
			toggleRatingStars(4);
		}




		private void OnRatingStar5Click(object sender, EventArgs args) {
			toggleRatingStars(5);
		}




		private void toggleRatingStars(int rating) {
			if (rating >= 2) {
				_layout.FindViewById<ImageView>(Resource.Id.rating_dialog_rating_star_2).SetImageResource(Resource.Drawable.ic_star_crop_full);
			} else {
				_layout.FindViewById<ImageView>(Resource.Id.rating_dialog_rating_star_2).SetImageResource(Resource.Drawable.ic_star_crop_empty);
			}


			if (rating >= 3) {
				_layout.FindViewById<ImageView>(Resource.Id.rating_dialog_rating_star_3).SetImageResource(Resource.Drawable.ic_star_crop_full);
			} else {
				_layout.FindViewById<ImageView>(Resource.Id.rating_dialog_rating_star_3).SetImageResource(Resource.Drawable.ic_star_crop_empty);
			}


			if (rating >= 4) {
				_layout.FindViewById<ImageView>(Resource.Id.rating_dialog_rating_star_4).SetImageResource(Resource.Drawable.ic_star_crop_full);
			} else {
				_layout.FindViewById<ImageView>(Resource.Id.rating_dialog_rating_star_4).SetImageResource(Resource.Drawable.ic_star_crop_empty);
			}


			if (rating >= 5) {
				_layout.FindViewById<ImageView>(Resource.Id.rating_dialog_rating_star_5).SetImageResource(Resource.Drawable.ic_star_crop_full);
			} else {
				_layout.FindViewById<ImageView>(Resource.Id.rating_dialog_rating_star_5).SetImageResource(Resource.Drawable.ic_star_crop_empty);
			}
		}




		public void showDialog() {
			_dialog.Show();
		}
	}
}