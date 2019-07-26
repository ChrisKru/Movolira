using Android.Animation;
using Android.App;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Newtonsoft.Json;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using v7ActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;

namespace Movolira {
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
	public class MainActivity : AppCompatActivity {
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
			Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.main_activity_toolbar);
			SetSupportActionBar(toolbar);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			_drawer_layout = FindViewById<DrawerLayout>(Resource.Id.main_activity_drawer_layout);
			_drawer_toggle = new v7ActionBarDrawerToggle(this, _drawer_layout, toolbar, Android.Resource.Drawable.IcMenuDirections,
				Android.Resource.Drawable.IcMenuDirections);
			_drawer_layout.AddDrawerListener(_drawer_toggle);
			LinearLayout menu = FindViewById<LinearLayout>(Resource.Id.main_activity_navigation_menu);
			LayoutTransition menu_transition = new LayoutTransition();
			menu_transition.EnableTransitionType(LayoutTransitionType.Appearing);
			menu_transition.EnableTransitionType(LayoutTransitionType.Disappearing);
			menu.LayoutTransition = menu_transition;
			MenuOnClickListener menu_on_click_listener = new MenuOnClickListener(menu);
			for (int i_menu_children = 0; i_menu_children < menu.ChildCount; ++i_menu_children) {
				menu.GetChildAt(i_menu_children).SetOnClickListener(menu_on_click_listener);
			}
			if (SupportFragmentManager.FindFragmentById(Resource.Id.main_activity_fragment_frame) == null) {
				SupportFragmentManager.BeginTransaction().Add(Resource.Id.main_activity_fragment_frame, new ShowListFragment(), null).Commit();
			}
		}

		protected override void OnPostCreate(Bundle saved_app_state) {
			_drawer_toggle.SyncState();
			base.OnPostCreate(saved_app_state);
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

		private class MenuOnClickListener : Object, View.IOnClickListener {
			private readonly LinearLayout _menu;

			public MenuOnClickListener(LinearLayout menu) {
				_menu = menu;
			}

			public void OnClick(View clicked_view) {
				if (clicked_view.Id == Resource.Id.menu_movies) {
					View menu_movies_popular = _menu.FindViewById(Resource.Id.menu_movies_popular);
					ImageView expandable_icon = _menu.FindViewById<ImageView>(Resource.Id.menu_movies_expandable_icon);
					if (menu_movies_popular.Visibility == ViewStates.Visible) {
						menu_movies_popular.Visibility = ViewStates.Gone;
						expandable_icon.SetImageResource(Resource.Mipmap.ic_expand_arrow);
					} else {
						menu_movies_popular.Visibility = ViewStates.Visible;
						expandable_icon.SetImageResource(Resource.Mipmap.ic_collapse_arrow);
					}
				} else if (clicked_view.Id == Resource.Id.menu_shows) {
					View menu_shows_popular = _menu.FindViewById(Resource.Id.menu_shows_popular);
					ImageView expandable_icon = _menu.FindViewById<ImageView>(Resource.Id.menu_shows_expandable_icon);
					if (menu_shows_popular.Visibility == ViewStates.Visible) {
						menu_shows_popular.Visibility = ViewStates.Gone;
						expandable_icon.SetImageResource(Resource.Mipmap.ic_expand_arrow);
					} else {
						menu_shows_popular.Visibility = ViewStates.Visible;
						expandable_icon.SetImageResource(Resource.Mipmap.ic_collapse_arrow);
					}
				}
			}
		}
	}
}