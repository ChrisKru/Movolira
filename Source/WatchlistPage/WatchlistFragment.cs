﻿using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace Movolira {
	public class WatchlistFragment : Fragment {
		private MainActivity _main_activity;




		public override void OnAttach(Context main_activity) {
			_main_activity = (MainActivity) main_activity;
			base.OnAttach(main_activity);
		}




		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle saved_instance_state) {
			View layout = inflater.Inflate(Resource.Layout.watchlist_page, container, false);


			ViewGroup entries_layout = layout.FindViewById<ViewGroup>(Resource.Id.watchlist_page_entries);
			var watchlist = _main_activity.UserData.getWatchlist();
			foreach (ShowSerialized show in watchlist) {
				ViewGroup watchlist_entry = (ViewGroup) inflater.Inflate(Resource.Layout.watchlist_page_entry, entries_layout, false);
				entries_layout.AddView(watchlist_entry);


				TextView watchlist_entry_title = watchlist_entry.FindViewById<TextView>(Resource.Id.watchlist_entry_title);
				TextView watchlist_entry_genre = watchlist_entry.FindViewById<TextView>(Resource.Id.watchlist_entry_genre);


				watchlist_entry_title.Text = show.Title;
				watchlist_entry_genre.Text = show.Genre;
				watchlist_entry.Click += (sender, args) => onWatchlistEntryClick(show);
			}


			_main_activity.setToolbarTitle("Watchlist");
			_main_activity.setIsLoading(false);
			return layout;
		}




		private void onWatchlistEntryClick(ShowSerialized show) {
			_main_activity.setIsLoading(true);
			moveToShowDetailsFrag(show);
		}




		private void moveToShowDetailsFrag(ShowSerialized show) {
			Fragment details_fragment;
			Bundle fragment_args = new Bundle();


			if (show.Type == ShowType.Movie.ToString()) {
				details_fragment = new MovieDetailsFragment();
				Movie movie = Movie.deserialize(show);
				fragment_args.PutString("movie", JsonConvert.SerializeObject(movie));
			} else {
				details_fragment = new TvShowDetailsFragment();
				TvShow tv_show = TvShow.deserialize(show);
				fragment_args.PutString("tv_show", JsonConvert.SerializeObject(tv_show));
			}


			details_fragment.Arguments = fragment_args;
			_main_activity.SupportFragmentManager.BeginTransaction().Replace(Resource.Id.main_activity_fragment_frame, details_fragment)
				.SetTransition(FragmentTransaction.TransitFragmentFade).AddToBackStack(null).Commit();
		}
	}
}