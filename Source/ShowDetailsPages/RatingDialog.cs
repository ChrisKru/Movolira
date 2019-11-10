using Android.App;

namespace Movolira {
	internal class RatingDialog {
		private AlertDialog _dialog;
		private MainActivity _main_activity;




		public RatingDialog(MainActivity main_activity) {
			_main_activity = main_activity;
		}




		public void showDialog() {
			_dialog.Show();
		}
	}
}