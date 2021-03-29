using Android.Animation;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using SearchView = Android.Support.V7.Widget.SearchView;
using Toolbar = Android.Support.V7.Widget.Toolbar;




namespace Movolira {
	public static class MainActivityViewBuilder {
		public static void buildLoadingView(MainActivity main_activity, out ImageView loading_view) {
			loading_view = main_activity.FindViewById<ImageView>(Resource.Id.show_list_loading);
			((AnimationDrawable)loading_view.Background).Start();
			main_activity.setIsLoading(true);
		}




		public static void buildToolbar(MainActivity main_activity, Bundle saved_instance_state, out Toolbar toolbar) {
			toolbar = main_activity.FindViewById<Toolbar>(Resource.Id.main_activity_toolbar);
			if (saved_instance_state != null) {
				toolbar.Title = saved_instance_state.GetString("toolbar_title");
			}
			main_activity.SetSupportActionBar(toolbar);
			main_activity.SupportActionBar.SetDisplayHomeAsUpEnabled(true);
		}




		public static void buildDrawerMenu(MainActivity main_activity, Toolbar toolbar,
			out DrawerLayout drawer_layout, out ActionBarDrawerToggle drawer_toggle) {


			drawer_layout = main_activity.FindViewById<DrawerLayout>(Resource.Id.main_activity_drawer_layout);
			drawer_toggle = new ActionBarDrawerToggle(main_activity, drawer_layout, toolbar,
				Android.Resource.Drawable.IcMenuDirections, Android.Resource.Drawable.IcMenuDirections);
			drawer_layout.AddDrawerListener(drawer_toggle);


			LinearLayout drawer_menu = main_activity.FindViewById<LinearLayout>(Resource.Id.main_activity_navigation_menu);
			LayoutTransition drawer_menu_transition = new LayoutTransition();
			drawer_menu_transition.DisableTransitionType(LayoutTransitionType.Disappearing);
			drawer_menu.LayoutTransition = drawer_menu_transition;
			MenuOnClickListener menu_on_click_listener = new MenuOnClickListener(main_activity, drawer_layout);
			for (int i_menu_children = 0; i_menu_children < drawer_menu.ChildCount; ++i_menu_children) {
				drawer_menu.GetChildAt(i_menu_children).SetOnClickListener(menu_on_click_listener);
			}
		}




		public static void buildSearchView(MainActivity main_activity, IMenu menu) {
			IMenuItem search_item = menu.FindItem(Resource.Id.toolbar_menu_search);
			SearchView search_view = (SearchView)search_item.ActionView;
			search_view.QueryHint = "Movie or Tv show";
			search_view.SetOnQueryTextListener(new SearchQueryTextListener(main_activity, search_item));
		}
	}
}