using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;




namespace Movolira.Pages.SettingsPage {
	public class SettingsFragment : Fragment {
		private MainActivity _main_activity;




		public override void OnAttach(Context main_activity) {
			this._main_activity = (MainActivity)main_activity;
			base.OnAttach(main_activity);
		}




		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle saved_instance_state) {
			View layout = inflater.Inflate(Resource.Layout.settings_page, container, false);
			Spinner theme_spinner = layout.FindViewById<Spinner>(Resource.Id.settings_page_theme_spinner);
			theme_spinner.Adapter = new ThemeSpinnerAdapter(this._main_activity);
			theme_spinner.SetSelection(this._main_activity.SettingsProvider.getCurrentThemeIndex(), false);
			theme_spinner.OnItemSelectedListener = new ThemeSpinnerOnItemSelectedListener(this._main_activity);


			this._main_activity.setToolbarTitle("Settings");
			this._main_activity.setIsLoading(false);
			return layout;
		}
	}
}