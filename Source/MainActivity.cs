using Android.App;
using Android.OS;
using Android.Support.V4.App;
using Newtonsoft.Json;

namespace Movolira {
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
	public class MainActivity : FragmentActivity {
		public DataProvider ShowDataProvider { get; private set; }

		protected override void OnCreate(Bundle saved_app_state) {
			base.OnCreate(saved_app_state);
			if (saved_app_state != null) {
				ShowDataProvider = JsonConvert.DeserializeObject<DataProvider>(saved_app_state.GetString("DataProvider"));
			} else {
				ShowDataProvider = new DataProvider();
			}
			SetContentView(Resource.Layout.main_activity);
			if (SupportFragmentManager.FindFragmentById(Resource.Id.main_activity_frame) == null) {
				SupportFragmentManager.BeginTransaction().Add(Resource.Id.main_activity_frame, new ShowListFragment(), null).Commit();
			}
		}

		public override void OnBackPressed() {
			if (SupportFragmentManager.BackStackEntryCount > 0) {
				SupportFragmentManager.PopBackStack();
			} else {
				base.OnBackPressed();
			}
		}

		protected override void OnSaveInstanceState(Bundle new_app_state) {
			new_app_state.PutString("DataProvider", JsonConvert.SerializeObject(ShowDataProvider));
			System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(ShowDataProvider));
			base.OnSaveInstanceState(new_app_state);
		}
	}
}