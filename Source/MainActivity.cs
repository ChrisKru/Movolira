using System.Threading.Tasks;
using Android.Animation;
using Android.App;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Newtonsoft.Json;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using v7ActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;

namespace Movolira {
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
	public class MainActivity : AppCompatActivity {
		public DataProvider DataProvider { get; private set; }
		public bool IsLoading { get; private set; }
		private DrawerLayout _drawer_layout;
		private v7ActionBarDrawerToggle _drawer_toggle;
		private ImageView _loading_view;

		public void setIsLoading(bool is_loading) {
			IsLoading = is_loading;
			if (is_loading) {
				_loading_view.Visibility = ViewStates.Visible;
			} else {
				Task.Delay(200).ContinueWith(a => RunOnUiThread(() => _loading_view.Visibility = ViewStates.Gone));
			}
		}

		public void changeContentFragment(string subtype) {
			RunOnUiThread(() => setIsLoading(true));
			ShowListFragment content_fragment = new ShowListFragment();
			Bundle fragment_args = new Bundle();
			fragment_args.PutString("subtype", subtype);
			content_fragment.Arguments = fragment_args;
			SupportFragmentManager.BeginTransaction().Replace(Resource.Id.main_activity_fragment_frame, content_fragment)
				.SetTransition(FragmentTransaction.TransitFragmentFade).AddToBackStack(null).Commit();
		}

		protected override void OnCreate(Bundle saved_app_state) {
			base.OnCreate(saved_app_state);
			if (saved_app_state != null) {
				DataProvider = JsonConvert.DeserializeObject<DataProvider>(saved_app_state.GetString("DataProvider"));
			} else {
				DataProvider = new DataProvider();
			}
			SetContentView(Resource.Layout.main_activity);
			_loading_view = FindViewById<ImageView>(Resource.Id.show_list_loading);
			((AnimationDrawable) _loading_view.Background).Start();
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
			MenuOnClickListener menu_on_click_listener = new MenuOnClickListener(this, _drawer_layout);
			for (int i_menu_children = 0; i_menu_children < menu.ChildCount; ++i_menu_children) {
				menu.GetChildAt(i_menu_children).SetOnClickListener(menu_on_click_listener);
			}
			if (SupportFragmentManager.FindFragmentById(Resource.Id.main_activity_fragment_frame) == null) {
				ShowListFragment content_frag = new ShowListFragment();
				Bundle frag_args = new Bundle();
				frag_args.PutString("subtype", "popular");
				content_frag.Arguments = frag_args;
				SupportFragmentManager.BeginTransaction().Add(Resource.Id.main_activity_fragment_frame, content_frag, null).Commit();
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
			} else{
				var fragments = SupportFragmentManager.Fragments;
				for (int i_fragment = 0; i_fragment < fragments.Count; ++i_fragment) {
					if (SupportFragmentManager.Fragments[i_fragment] is IBackButtonHandler back_button_handler) {
						if (back_button_handler.handleBackButtonPress()) {
							return;
						}
					}
				}
				base.OnBackPressed();
			}
		}

		protected override void OnSaveInstanceState(Bundle new_app_state) {
			new_app_state.PutString("DataProvider", JsonConvert.SerializeObject(DataProvider));
			base.OnSaveInstanceState(new_app_state);
		}

		private class MenuOnClickListener : Object, View.IOnClickListener {
			private readonly DrawerLayout _drawer;
			private readonly MainActivity _main_activity;

			public MenuOnClickListener(MainActivity main_activity, DrawerLayout drawer) {
				_main_activity = main_activity;
				_drawer = drawer;
			}

			public void OnClick(View clicked_view) {
				if (clicked_view.Id == Resource.Id.menu_movies) {
					toggleMoviesGroup();
				} else if (clicked_view.Id == Resource.Id.menu_shows) {
					toggleShowsGroup();
				} else if (clicked_view.Id == Resource.Id.menu_movies_popular) {
					_drawer.CloseDrawer(GravityCompat.Start);
					collapseAllGroups();
					Task.Run(() => _main_activity.changeContentFragment("popular"));
				} else if (clicked_view.Id == Resource.Id.menu_movies_trending) {
					_drawer.CloseDrawer(GravityCompat.Start);
					collapseAllGroups();
					Task.Run(() => _main_activity.changeContentFragment("trending"));
				}
			}

			private void collapseAllGroups() {
				_drawer.FindViewById<ImageView>(Resource.Id.menu_movies_expandable_icon).SetImageResource(Resource.Mipmap.ic_expand_arrow);
				_drawer.FindViewById(Resource.Id.menu_movies_popular).Visibility = ViewStates.Gone;
				_drawer.FindViewById(Resource.Id.menu_movies_trending).Visibility = ViewStates.Gone;
				_drawer.FindViewById<ImageView>(Resource.Id.menu_shows_expandable_icon).SetImageResource(Resource.Mipmap.ic_expand_arrow);
				_drawer.FindViewById(Resource.Id.menu_shows_popular).Visibility = ViewStates.Gone;
			}

			private void toggleMoviesGroup() {
				View menu_movies_popular = _drawer.FindViewById(Resource.Id.menu_movies_popular);
				ImageView expandable_icon = _drawer.FindViewById<ImageView>(Resource.Id.menu_movies_expandable_icon);
				if (menu_movies_popular.Visibility == ViewStates.Visible) {
					menu_movies_popular.Visibility = ViewStates.Gone;
					_drawer.FindViewById(Resource.Id.menu_movies_trending).Visibility = ViewStates.Gone;
					expandable_icon.SetImageResource(Resource.Mipmap.ic_expand_arrow);
				} else {
					menu_movies_popular.Visibility = ViewStates.Visible;
					_drawer.FindViewById(Resource.Id.menu_movies_trending).Visibility = ViewStates.Visible;
					expandable_icon.SetImageResource(Resource.Mipmap.ic_collapse_arrow);
				}
			}

			private void toggleShowsGroup() {
				View menu_shows_popular = _drawer.FindViewById(Resource.Id.menu_shows_popular);
				ImageView expandable_icon = _drawer.FindViewById<ImageView>(Resource.Id.menu_shows_expandable_icon);
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