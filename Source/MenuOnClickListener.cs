using System.Threading.Tasks;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using Java.Lang;




namespace Movolira {
	public class MenuOnClickListener : Object, View.IOnClickListener {
		private DrawerLayout _drawer;
		private MainActivity _main_activity;




		public MenuOnClickListener(MainActivity main_activity, DrawerLayout drawer) {
			this._main_activity = main_activity;
			this._drawer = drawer;
		}




		public void OnClick(View clicked_view) {
			if (clicked_view.Id == Resource.Id.menu_movies) {
				this.toggleMoviesGroup();
			} else if (clicked_view.Id == Resource.Id.menu_tv_shows) {
				this.toggleShowsGroup();


			} else if (clicked_view.Id == Resource.Id.menu_discover) {
				this._drawer.CloseDrawer(GravityCompat.Start);
				this.collapseAllGroups();
				Task.Run(() => this._main_activity.changeContentFragment("discover", ""));


			} else if (clicked_view.Id == Resource.Id.menu_watchlist) {
				this._drawer.CloseDrawer(GravityCompat.Start);
				this.collapseAllGroups();
				Task.Run(() => this._main_activity.changeContentFragment("watchlist", ""));


			} else if (clicked_view.Id == Resource.Id.menu_rated_shows) {
				this._drawer.CloseDrawer(GravityCompat.Start);
				this.collapseAllGroups();
				Task.Run(() => this._main_activity.changeContentFragment("rated_shows", ""));


			} else if (clicked_view.Id == Resource.Id.menu_recommendations) {
				this._drawer.CloseDrawer(GravityCompat.Start);
				this.collapseAllGroups();
				Task.Run(() => this._main_activity.moveToRecommendationFragment());


			} else if (clicked_view.Id == Resource.Id.menu_settings) {
				this._drawer.CloseDrawer(GravityCompat.Start);
				this.collapseAllGroups();
				Task.Run(() => this._main_activity.changeContentFragment("settings", ""));


			} else {
				if (!this.checkMovieMenusForClickEvents(clicked_view)) {
					this.checkShowsMenusForClickEvents(clicked_view);
				}
			}
		}




		private bool checkMovieMenusForClickEvents(View clicked_view) {
			if (clicked_view.Id == Resource.Id.menu_movies_popular) {
				this._drawer.CloseDrawer(GravityCompat.Start);
				this.collapseAllGroups();
				Task.Run(() => this._main_activity.changeContentFragment("movies", "popular"));


			} else if (clicked_view.Id == Resource.Id.menu_movies_top_rated) {
				this._drawer.CloseDrawer(GravityCompat.Start);
				this.collapseAllGroups();
				Task.Run(() => this._main_activity.changeContentFragment("movies", "top_rated"));


			} else if (clicked_view.Id == Resource.Id.menu_movies_upcoming) {
				this._drawer.CloseDrawer(GravityCompat.Start);
				this.collapseAllGroups();
				Task.Run(() => this._main_activity.changeContentFragment("movies", "upcoming"));


			} else {
				return false;
			}
			return true;
		}




		private bool checkShowsMenusForClickEvents(View clicked_view) {
			if (clicked_view.Id == Resource.Id.menu_tv_shows_popular) {
				this._drawer.CloseDrawer(GravityCompat.Start);
				this.collapseAllGroups();
				Task.Run(() => this._main_activity.changeContentFragment("tv_shows", "popular"));


			} else if (clicked_view.Id == Resource.Id.menu_tv_shows_top_rated) {
				this._drawer.CloseDrawer(GravityCompat.Start);
				this.collapseAllGroups();
				Task.Run(() => this._main_activity.changeContentFragment("tv_shows", "top_rated"));


			} else {
				return false;
			}
			return true;
		}




		private void collapseAllGroups() {
			this._drawer.FindViewById<ImageView>(Resource.Id.menu_movies_expandable_icon)
				.SetImageResource(Resource.Drawable.ic_expand_arrow);
			this._drawer.FindViewById(Resource.Id.menu_movies_popular).Visibility = ViewStates.Gone;
			this._drawer.FindViewById(Resource.Id.menu_movies_top_rated).Visibility = ViewStates.Gone;
			this._drawer.FindViewById(Resource.Id.menu_movies_upcoming).Visibility = ViewStates.Gone;
			this._drawer.FindViewById<ImageView>(Resource.Id.menu_tv_shows_expandable_icon)
				.SetImageResource(Resource.Drawable.ic_expand_arrow);
			this._drawer.FindViewById(Resource.Id.menu_tv_shows_popular).Visibility = ViewStates.Gone;
			this._drawer.FindViewById(Resource.Id.menu_tv_shows_top_rated).Visibility = ViewStates.Gone;
		}




		private void toggleMoviesGroup() {
			View menu_movies_popular = this._drawer.FindViewById(Resource.Id.menu_movies_popular);
			ImageView expandable_icon = this._drawer.FindViewById<ImageView>(Resource.Id.menu_movies_expandable_icon);


			if (menu_movies_popular.Visibility == ViewStates.Visible) {
				menu_movies_popular.Visibility = ViewStates.Invisible;
				this._drawer.FindViewById(Resource.Id.menu_movies_top_rated).Visibility = ViewStates.Invisible;
				this._drawer.FindViewById(Resource.Id.menu_movies_upcoming).Visibility = ViewStates.Invisible;
				menu_movies_popular.Visibility = ViewStates.Gone;
				this._drawer.FindViewById(Resource.Id.menu_movies_top_rated).Visibility = ViewStates.Gone;
				this._drawer.FindViewById(Resource.Id.menu_movies_upcoming).Visibility = ViewStates.Gone;
				expandable_icon.SetImageResource(Resource.Drawable.ic_expand_arrow);


			} else {
				menu_movies_popular.Visibility = ViewStates.Invisible;
				this._drawer.FindViewById(Resource.Id.menu_movies_top_rated).Visibility = ViewStates.Invisible;
				this._drawer.FindViewById(Resource.Id.menu_movies_upcoming).Visibility = ViewStates.Invisible;
				menu_movies_popular.Visibility = ViewStates.Visible;
				this._drawer.FindViewById(Resource.Id.menu_movies_top_rated).Visibility = ViewStates.Visible;
				this._drawer.FindViewById(Resource.Id.menu_movies_upcoming).Visibility = ViewStates.Visible;
				expandable_icon.SetImageResource(Resource.Drawable.ic_collapse_arrow);
			}
		}




		private void toggleShowsGroup() {
			View menu_tv_shows_popular = this._drawer.FindViewById(Resource.Id.menu_tv_shows_popular);
			ImageView expandable_icon = this._drawer.FindViewById<ImageView>(Resource.Id.menu_tv_shows_expandable_icon);


			if (menu_tv_shows_popular.Visibility == ViewStates.Visible) {
				menu_tv_shows_popular.Visibility = ViewStates.Invisible;
				this._drawer.FindViewById(Resource.Id.menu_tv_shows_top_rated).Visibility = ViewStates.Invisible;
				menu_tv_shows_popular.Visibility = ViewStates.Gone;
				this._drawer.FindViewById(Resource.Id.menu_tv_shows_top_rated).Visibility = ViewStates.Gone;
				expandable_icon.SetImageResource(Resource.Drawable.ic_expand_arrow);


			} else {
				menu_tv_shows_popular.Visibility = ViewStates.Invisible;
				this._drawer.FindViewById(Resource.Id.menu_tv_shows_top_rated).Visibility = ViewStates.Invisible;
				menu_tv_shows_popular.Visibility = ViewStates.Visible;
				this._drawer.FindViewById(Resource.Id.menu_tv_shows_top_rated).Visibility = ViewStates.Visible;
				expandable_icon.SetImageResource(Resource.Drawable.ic_collapse_arrow);
			}
		}
	}
}