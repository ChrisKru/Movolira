﻿using System.Threading.Tasks;
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
			} else if (clicked_view.Id == Resource.Id.menu_movies_trending) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("trending"));
			} else if (clicked_view.Id == Resource.Id.menu_movies_most_popular) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("most_popular"));
			} else if (clicked_view.Id == Resource.Id.menu_movies_most_watched) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("most_watched"));
			} else if (clicked_view.Id == Resource.Id.menu_movies_most_collected) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("most_collected"));
			} else if (clicked_view.Id == Resource.Id.menu_movies_most_anticipated) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("most_anticipated"));
			} else if (clicked_view.Id == Resource.Id.menu_movies_box_office) {
				_drawer.CloseDrawer(GravityCompat.Start);
				collapseAllGroups();
				Task.Run(() => _main_activity.changeContentFragment("box_office"));
			}
		}

		private void collapseAllGroups() {
			_drawer.FindViewById<ImageView>(Resource.Id.menu_movies_expandable_icon).SetImageResource(Resource.Mipmap.ic_expand_arrow);
			_drawer.FindViewById(Resource.Id.menu_movies_trending).Visibility = ViewStates.Gone;
			_drawer.FindViewById(Resource.Id.menu_movies_most_popular).Visibility = ViewStates.Gone;
			_drawer.FindViewById(Resource.Id.menu_movies_most_watched).Visibility = ViewStates.Gone;
			_drawer.FindViewById(Resource.Id.menu_movies_most_collected).Visibility = ViewStates.Gone;
			_drawer.FindViewById(Resource.Id.menu_movies_most_anticipated).Visibility = ViewStates.Gone;
			_drawer.FindViewById(Resource.Id.menu_movies_box_office).Visibility = ViewStates.Gone;
			_drawer.FindViewById<ImageView>(Resource.Id.menu_shows_expandable_icon).SetImageResource(Resource.Mipmap.ic_expand_arrow);
			_drawer.FindViewById(Resource.Id.menu_shows_popular).Visibility = ViewStates.Gone;
		}

		private void toggleMoviesGroup() {
			View menu_movies_trending = _drawer.FindViewById(Resource.Id.menu_movies_trending);
			ImageView expandable_icon = _drawer.FindViewById<ImageView>(Resource.Id.menu_movies_expandable_icon);
			if (menu_movies_trending.Visibility == ViewStates.Visible) {
				menu_movies_trending.Visibility = ViewStates.Gone;
				_drawer.FindViewById(Resource.Id.menu_movies_most_popular).Visibility = ViewStates.Gone;
				_drawer.FindViewById(Resource.Id.menu_movies_most_watched).Visibility = ViewStates.Gone;
				_drawer.FindViewById(Resource.Id.menu_movies_most_collected).Visibility = ViewStates.Gone;
				_drawer.FindViewById(Resource.Id.menu_movies_most_anticipated).Visibility = ViewStates.Gone;
				_drawer.FindViewById(Resource.Id.menu_movies_box_office).Visibility = ViewStates.Gone;
				expandable_icon.SetImageResource(Resource.Mipmap.ic_expand_arrow);
			} else {
				menu_movies_trending.Visibility = ViewStates.Visible;
				_drawer.FindViewById(Resource.Id.menu_movies_most_popular).Visibility = ViewStates.Visible;
				_drawer.FindViewById(Resource.Id.menu_movies_most_watched).Visibility = ViewStates.Visible;
				_drawer.FindViewById(Resource.Id.menu_movies_most_collected).Visibility = ViewStates.Visible;
				_drawer.FindViewById(Resource.Id.menu_movies_most_anticipated).Visibility = ViewStates.Visible;
				_drawer.FindViewById(Resource.Id.menu_movies_box_office).Visibility = ViewStates.Visible;
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