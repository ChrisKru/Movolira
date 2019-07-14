using Android.App;
using Android.OS;
using Android.Support.V4.App;
using Newtonsoft.Json;

namespace Movolira {
	[Activity(
		Label = "@string/app_name",
		Theme = "@style/AppTheme",
		MainLauncher = true)]
	public class MainActivity : FragmentActivity {
		public MovieDataProvider MovieDataProvider { get; private set; }

		protected override void OnCreate(Bundle saved_app_state) {
			base.OnCreate(saved_app_state);
			if (saved_app_state != null)
				MovieDataProvider = JsonConvert.DeserializeObject<MovieDataProvider>(saved_app_state.GetString("MovieDataProvider"));
			else
				MovieDataProvider = new MovieDataProvider();
			SetContentView(Resource.Layout.main_activity);
			if (SupportFragmentManager.FindFragmentById(Resource.Id.main_activity_frame) == null)
				SupportFragmentManager.BeginTransaction().Add(Resource.Id.main_activity_frame, new MovieListFragment(), null).Commit();
		}

		public override void OnBackPressed() {
			if (SupportFragmentManager.BackStackEntryCount > 0)
				SupportFragmentManager.PopBackStack();
			else
				base.OnBackPressed();
		}

		protected override void OnSaveInstanceState(Bundle new_app_state) {
			new_app_state.PutString("MovieDataProvider", JsonConvert.SerializeObject(MovieDataProvider));
			base.OnSaveInstanceState(new_app_state);
		}
	}
}