using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Movolira {
	internal class RatingButtonClickListener : Object, Android.Views.View.IOnClickListener {
		private RatingDialog _dialog;
		private MainActivity _main_activity;




		public RatingButtonClickListener(MainActivity main_activity) {
			_main_activity = main_activity;
			_dialog = new RatingDialog(_main_activity);
		}




		public void OnClick(View view) {
			_dialog.showDialog();
		}
	}
}