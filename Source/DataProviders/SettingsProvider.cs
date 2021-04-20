using System.Linq;
using Android.Content;
using Realms;




namespace Movolira.DataProviders {
	public class SettingsProvider {
		private readonly int[] THEME_RIDS = {
			Resource.Style.AppThemeBlue,
			Resource.Style.AppThemeViolet
		};
		private MainActivity _main_activity;
		private Settings _settings;




		public SettingsProvider(MainActivity main_activity) {
			this._main_activity = main_activity;
			Realm realm_db = Realm.GetInstance();
			this._settings = realm_db.All<Settings>().FirstOrDefault();


			if (this._settings == null) {
				this._settings = new Settings();
				realm_db.Write(() => realm_db.Add(this._settings));
			}


			this._main_activity.SetTheme(this.THEME_RIDS[this._settings.CurrentThemeIndex]);
		}




		public int getCurrentThemeIndex() {
			return this._settings.CurrentThemeIndex;
		}




		public void setCurrentThemeIndex(int new_theme_index) {
			Realm realm_db = Realm.GetInstance();
			realm_db.Write(() => {
				this._settings.CurrentThemeIndex = new_theme_index;
			});


			// The activity needs to be recreated for the theme change to be applied.
			Intent activity_intent = this._main_activity.Intent;
			activity_intent.PutExtra("was_restarted_by_settings_page", true);
			this._main_activity.Finish();
			this._main_activity.StartActivity(activity_intent);
		}
	}
}