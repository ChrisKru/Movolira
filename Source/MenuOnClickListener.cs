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
			} else if (clicked_view.Id == Resource.Id.menu_tv_shows) {
				toggleShowsGroup();
			} else if (clicked_view.Id == Resource.Id.menu_discover) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("discover", ""));
			} else if (clicked_view.Id == Resource.Id.menu_watchlist) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("watchlist", ""));
			} else {
				if (!checkMovieMenusForClickEvents(clicked_view)) {
					checkShowsMenusForClickEvents(clicked_view);
				}
			}
		}




		private bool checkMovieMenusForClickEvents(View clicked_view) {
			if (clicked_view.Id == Resource.Id.menu_movies_popular) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("movies", "popular"));
			} else if (clicked_view.Id == Resource.Id.menu_movies_top_rated) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("movies", "top_rated"));
			} else if (clicked_view.Id == Resource.Id.menu_movies_upcoming) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("movies", "upcoming"));
			} else {
				return false;
			}


			return true;
		}




		private bool checkShowsMenusForClickEvents(View clicked_view) {
			if (clicked_view.Id == Resource.Id.menu_tv_shows_popular) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("tv_shows", "popular"));
			} else if (clicked_view.Id == Resource.Id.menu_tv_shows_top_rated) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("tv_shows", "top_rated"));
			} else {
				return false;
			}
			return true;
		}




		private void collapseAllGroups() {
			_drawer.FindViewById<ImageView>(Resource.Id.menu_movies_expandable_icon).SetImageResource(Resource.Drawable.ic_expand_arrow);
			_drawer.FindViewById(Resource.Id.menu_movies_popular).Visibility = ViewStates.Gone;
			_drawer.FindViewById(Resource.Id.menu_movies_top_rated).Visibility = ViewStates.Gone;
			_drawer.FindViewById(Resource.Id.menu_movies_upcoming).Visibility = ViewStates.Gone;


			_drawer.FindViewById<ImageView>(Resource.Id.menu_tv_shows_expandable_icon).SetImageResource(Resource.Drawable.ic_expand_arrow);
			_drawer.FindViewById(Resource.Id.menu_tv_shows_popular).Visibility = ViewStates.Gone;
			_drawer.FindViewById(Resource.Id.menu_tv_shows_top_rated).Visibility = ViewStates.Gone;
		}




		private void toggleMoviesGroup() {
			View menu_movies_popular = _drawer.FindViewById(Resource.Id.menu_movies_popular);
			ImageView expandable_icon = _drawer.FindViewById<ImageView>(Resource.Id.menu_movies_expandable_icon);
			if (menu_movies_popular.Visibility == ViewStates.Visible) {
				menu_movies_popular.Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_movies_top_rated).Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_movies_upcoming).Visibility = ViewStates.Invisible;


				menu_movies_popular.Visibility = ViewStates.Gone;
				_drawer.FindViewById(Resource.Id.menu_movies_top_rated).Visibility = ViewStates.Gone;
				_drawer.FindViewById(Resource.Id.menu_movies_upcoming).Visibility = ViewStates.Gone;


				expandable_icon.SetImageResource(Resource.Drawable.ic_expand_arrow);
			} else {
				menu_movies_popular.Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_movies_top_rated).Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_movies_upcoming).Visibility = ViewStates.Invisible;


				menu_movies_popular.Visibility = ViewStates.Visible;
				_drawer.FindViewById(Resource.Id.menu_movies_top_rated).Visibility = ViewStates.Visible;
				_drawer.FindViewById(Resource.Id.menu_movies_upcoming).Visibility = ViewStates.Visible;


				expandable_icon.SetImageResource(Resource.Drawable.ic_collapse_arrow);
			}
		}




		private void toggleShowsGroup() {
			View menu_tv_shows_popular = _drawer.FindViewById(Resource.Id.menu_tv_shows_popular);
			ImageView expandable_icon = _drawer.FindViewById<ImageView>(Resource.Id.menu_tv_shows_expandable_icon);
			if (menu_tv_shows_popular.Visibility == ViewStates.Visible) {
				menu_tv_shows_popular.Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_tv_shows_top_rated).Visibility = ViewStates.Invisible;
				menu_tv_shows_popular.Visibility = ViewStates.Gone;
				_drawer.FindViewById(Resource.Id.menu_tv_shows_top_rated).Visibility = ViewStates.Gone;


				expandable_icon.SetImageResource(Resource.Drawable.ic_expand_arrow);
			} else {
				menu_tv_shows_popular.Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_tv_shows_top_rated).Visibility = ViewStates.Invisible;
				menu_tv_shows_popular.Visibility = ViewStates.Visible;
				_drawer.FindViewById(Resource.Id.menu_tv_shows_top_rated).Visibility = ViewStates.Visible;


				expandable_icon.SetImageResource(Resource.Drawable.ic_collapse_arrow);
			}
		}
	}
}