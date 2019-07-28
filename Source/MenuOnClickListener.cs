using System.Threading.Tasks;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Movolira {
	public class MenuOnClickListener : Object, View.IOnClickListener {
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