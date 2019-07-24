using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Newtonsoft.Json;
using v7ActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;

namespace Movolira {
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
	public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener {
		public DataProvider DataProvider { get; private set; }
		private DrawerLayout _drawer_layout;
		private v7ActionBarDrawerToggle _drawer_toggle;

		protected override void OnCreate(Bundle saved_app_state) {
			base.OnCreate(saved_app_state);
			if (saved_app_state != null) {
				DataProvider = JsonConvert.DeserializeObject<DataProvider>(saved_app_state.GetString("DataProvider"));
			} else {
				DataProvider = new DataProvider();
			}
			SetContentView(Resource.Layout.main_activity);
			Toolbar navigation_toolbar = FindViewById<Toolbar>(Resource.Id.navigation_toolbar);
			SetSupportActionBar(navigation_toolbar);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			_drawer_layout = FindViewById<DrawerLayout>(Resource.Id.main_activity_layout);
			_drawer_toggle = new v7ActionBarDrawerToggle(this, _drawer_layout, navigation_toolbar,
				Android.Resource.Drawable.IcMenuDirections, Android.Resource.Drawable.IcMenuDirections);
			_drawer_layout.AddDrawerListener(_drawer_toggle);
			NavigationView navigation_view = FindViewById<NavigationView>(Resource.Id.main_activity_navigation);
			navigation_view.SetNavigationItemSelectedListener(this);
			if (SupportFragmentManager.FindFragmentById(Resource.Id.main_activity_frame) == null) {
				SupportFragmentManager.BeginTransaction().Add(Resource.Id.main_activity_frame, new ShowListFragment(), null).Commit();
			}
		}

		protected override void OnPostCreate(Bundle saved_app_state) {
			_drawer_toggle.SyncState();
			base.OnPostCreate(saved_app_state);
		}

		public bool OnNavigationItemSelected(IMenuItem menu_item) {
			_drawer_layout.CloseDrawer(GravityCompat.Start);
			int menu_item_id = menu_item.ItemId;
			if (menu_item_id == Resource.Id.navigation_menu_movies) {


				return true;
			}
			if (menu_item_id == Resource.Id.navigation_menu_shows) {


				return true;
			}
			return false;
		}

		public override void OnBackPressed() {
			if (_drawer_layout.IsDrawerOpen(GravityCompat.Start)) {
				_drawer_layout.CloseDrawer(GravityCompat.Start);
			} else if (SupportFragmentManager.BackStackEntryCount > 0) {
				SupportFragmentManager.PopBackStack();
			} else {
				base.OnBackPressed();
			}
		}

		protected override void OnSaveInstanceState(Bundle new_app_state) {
			new_app_state.PutString("DataProvider", JsonConvert.SerializeObject(DataProvider));
			base.OnSaveInstanceState(new_app_state);
		}

		
	}
}