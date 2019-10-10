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
			} else {
				if (!checkMovieMenusForClickEvents(clicked_view)) {
					checkShowsMenusForClickEvents(clicked_view);
				}
			}
		}

		private bool checkMovieMenusForClickEvents(View clicked_view) {
			if (clicked_view.Id == Resource.Id.menu_movies_trending) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("movies", "trending"));
			} else if (clicked_view.Id == Resource.Id.menu_movies_most_popular) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("movies", "popular"));
			} else if (clicked_view.Id == Resource.Id.menu_movies_most_watched) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("movies", "watched"));
			} else if (clicked_view.Id == Resource.Id.menu_movies_most_collected) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("movies", "collected"));
			} else if (clicked_view.Id == Resource.Id.menu_movies_most_anticipated) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("movies", "anticipated"));
			} else if (clicked_view.Id == Resource.Id.menu_movies_box_office) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("movies", "box_office"));


			} else {
				return false;
			}


			return true;
		}

		private bool checkShowsMenusForClickEvents(View clicked_view) {
			if (clicked_view.Id == Resource.Id.menu_tv_shows_trending) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("tv_shows", "trending"));
			} else if (clicked_view.Id == Resource.Id.menu_tv_shows_most_popular) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("tv_shows", "popular"));
			} else if (clicked_view.Id == Resource.Id.menu_tv_shows_most_watched) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("tv_shows", "watched"));
			} else if (clicked_view.Id == Resource.Id.menu_tv_shows_most_collected) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("tv_shows", "collected"));
			} else if (clicked_view.Id == Resource.Id.menu_tv_shows_most_anticipated) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("tv_shows", "anticipated"));


			} else {
				return false;
			}

			return true;
		}


		private void collapseAllGroups() {
			_drawer.FindViewById<ImageView>(Resource.Id.menu_movies_expandable_icon).SetImageResource(Resource.Drawable.ic_expand_arrow);
			_drawer.FindViewById(Resource.Id.menu_movies_trending).Visibility = ViewStates.Gone;
			_drawer.FindViewById(Resource.Id.menu_movies_most_popular).Visibility = ViewStates.Gone;
			_drawer.FindViewById(Resource.Id.menu_movies_most_watched).Visibility = ViewStates.Gone;
			_drawer.FindViewById(Resource.Id.menu_movies_most_collected).Visibility = ViewStates.Gone;
			_drawer.FindViewById(Resource.Id.menu_movies_most_anticipated).Visibility = ViewStates.Gone;
			_drawer.FindViewById(Resource.Id.menu_movies_box_office).Visibility = ViewStates.Gone;


			_drawer.FindViewById<ImageView>(Resource.Id.menu_tv_shows_expandable_icon).SetImageResource(Resource.Drawable.ic_expand_arrow);
			_drawer.FindViewById(Resource.Id.menu_tv_shows_trending).Visibility = ViewStates.Gone;
			_drawer.FindViewById(Resource.Id.menu_tv_shows_most_popular).Visibility = ViewStates.Gone;
			_drawer.FindViewById(Resource.Id.menu_tv_shows_most_watched).Visibility = ViewStates.Gone;
			_drawer.FindViewById(Resource.Id.menu_tv_shows_most_collected).Visibility = ViewStates.Gone;
			_drawer.FindViewById(Resource.Id.menu_tv_shows_most_anticipated).Visibility = ViewStates.Gone;
		}


		private void toggleMoviesGroup() {
			View menu_movies_trending = _drawer.FindViewById(Resource.Id.menu_movies_trending);
			ImageView expandable_icon = _drawer.FindViewById<ImageView>(Resource.Id.menu_movies_expandable_icon);
			if (menu_movies_trending.Visibility == ViewStates.Visible) {
				menu_movies_trending.Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_movies_most_popular).Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_movies_most_watched).Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_movies_most_collected).Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_movies_most_anticipated).Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_movies_box_office).Visibility = ViewStates.Invisible;


				menu_movies_trending.Visibility = ViewStates.Gone;
				_drawer.FindViewById(Resource.Id.menu_movies_most_popular).Visibility = ViewStates.Gone;
				_drawer.FindViewById(Resource.Id.menu_movies_most_watched).Visibility = ViewStates.Gone;
				_drawer.FindViewById(Resource.Id.menu_movies_most_collected).Visibility = ViewStates.Gone;
				_drawer.FindViewById(Resource.Id.menu_movies_most_anticipated).Visibility = ViewStates.Gone;
				_drawer.FindViewById(Resource.Id.menu_movies_box_office).Visibility = ViewStates.Gone;


				expandable_icon.SetImageResource(Resource.Drawable.ic_expand_arrow);


			} else {
				menu_movies_trending.Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_movies_most_popular).Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_movies_most_watched).Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_movies_most_collected).Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_movies_most_anticipated).Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_movies_box_office).Visibility = ViewStates.Invisible;


				menu_movies_trending.Visibility = ViewStates.Visible;
				_drawer.FindViewById(Resource.Id.menu_movies_most_popular).Visibility = ViewStates.Visible;
				_drawer.FindViewById(Resource.Id.menu_movies_most_watched).Visibility = ViewStates.Visible;
				_drawer.FindViewById(Resource.Id.menu_movies_most_collected).Visibility = ViewStates.Visible;
				_drawer.FindViewById(Resource.Id.menu_movies_most_anticipated).Visibility = ViewStates.Visible;
				_drawer.FindViewById(Resource.Id.menu_movies_box_office).Visibility = ViewStates.Visible;


				expandable_icon.SetImageResource(Resource.Drawable.ic_collapse_arrow);
			}
		}


		private void toggleShowsGroup() {
			View menu_tv_shows_trending = _drawer.FindViewById(Resource.Id.menu_tv_shows_trending);
			ImageView expandable_icon = _drawer.FindViewById<ImageView>(Resource.Id.menu_tv_shows_expandable_icon);
			if (menu_tv_shows_trending.Visibility == ViewStates.Visible) {
				menu_tv_shows_trending.Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_tv_shows_most_popular).Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_tv_shows_most_watched).Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_tv_shows_most_collected).Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_tv_shows_most_anticipated).Visibility = ViewStates.Invisible;


				menu_tv_shows_trending.Visibility = ViewStates.Gone;
				_drawer.FindViewById(Resource.Id.menu_tv_shows_most_popular).Visibility = ViewStates.Gone;
				_drawer.FindViewById(Resource.Id.menu_tv_shows_most_watched).Visibility = ViewStates.Gone;
				_drawer.FindViewById(Resource.Id.menu_tv_shows_most_collected).Visibility = ViewStates.Gone;
				_drawer.FindViewById(Resource.Id.menu_tv_shows_most_anticipated).Visibility = ViewStates.Gone;


				expandable_icon.SetImageResource(Resource.Drawable.ic_expand_arrow);
			} else {
				menu_tv_shows_trending.Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_tv_shows_most_popular).Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_tv_shows_most_watched).Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_tv_shows_most_collected).Visibility = ViewStates.Invisible;
				_drawer.FindViewById(Resource.Id.menu_tv_shows_most_anticipated).Visibility = ViewStates.Invisible;


				menu_tv_shows_trending.Visibility = ViewStates.Visible;
				_drawer.FindViewById(Resource.Id.menu_tv_shows_most_popular).Visibility = ViewStates.Visible;
				_drawer.FindViewById(Resource.Id.menu_tv_shows_most_watched).Visibility = ViewStates.Visible;
				_drawer.FindViewById(Resource.Id.menu_tv_shows_most_collected).Visibility = ViewStates.Visible;
				_drawer.FindViewById(Resource.Id.menu_tv_shows_most_anticipated).Visibility = ViewStates.Visible;


				expandable_icon.SetImageResource(Resource.Drawable.ic_collapse_arrow);
			}
		}
	}
}