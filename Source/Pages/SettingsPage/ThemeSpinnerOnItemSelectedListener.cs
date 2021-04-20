using Android.Views;
using Android.Widget;
using static Android.Widget.AdapterView;
using Object = Java.Lang.Object;




namespace Movolira.Pages.SettingsPage {
	public class ThemeSpinnerOnItemSelectedListener : Object, IOnItemSelectedListener {
		private bool _is_initialized = false;
		private MainActivity _main_activity;




		public ThemeSpinnerOnItemSelectedListener(MainActivity main_activity) {
			this._main_activity = main_activity;
		}




		public void OnItemSelected(AdapterView parent_view, View item_view, int position, long item_id) {
			if (!this._is_initialized) {
				this._is_initialized = true;
				return;
			}


			if (position == this._main_activity.SettingsProvider.getCurrentThemeIndex()) {
				return;
			} else {
				this._main_activity.SettingsProvider.setCurrentThemeIndex(position);
			}
		}




		public void OnNothingSelected(AdapterView parent) { }
	}
}