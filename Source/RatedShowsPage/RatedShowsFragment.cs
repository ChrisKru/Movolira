﻿using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace Movolira {
	public class RatedShowsFragment : Fragment {
		private MainActivity _main_activity;




		public override void OnAttach(Context main_activity) {
			_main_activity = (MainActivity)main_activity;
			base.OnAttach(main_activity);
		}




		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle saved_instance_state) {
			View layout = inflater.Inflate(Resource.Layout.rated_shows_page, container, false);


			ViewGroup entries_layout = layout.FindViewById<ViewGroup>(Resource.Id.rated_shows_page_entries);
			var rated_shows = _main_activity.UserData.getRatedShows();
			foreach (RatedShowSerialized show in rated_shows) {
				ViewGroup rated_shows_entry = (ViewGroup)inflater.Inflate(Resource.Layout.rated_shows_page_entry, entries_layout, false);
				entries_layout.AddView(rated_shows_entry);


				TextView rated_shows_entry_title = rated_shows_entry.FindViewById<TextView>(Resource.Id.rated_shows_entry_title);
				rated_shows_entry_title.Text = show.Title;


				if (show.Rating > 1) {
					rated_shows_entry.FindViewById<ImageView>(Resource.Id.rated_shows_entry_rating_star_2).Visibility = ViewStates.Visible;
				}
				if (show.Rating > 2) {
					rated_shows_entry.FindViewById<ImageView>(Resource.Id.rated_shows_entry_rating_star_3).Visibility = ViewStates.Visible;
				}
				if (show.Rating > 3) {
					rated_shows_entry.FindViewById<ImageView>(Resource.Id.rated_shows_entry_rating_star_4).Visibility = ViewStates.Visible;
				}
				if (show.Rating > 4) {
					rated_shows_entry.FindViewById<ImageView>(Resource.Id.rated_shows_entry_rating_star_5).Visibility = ViewStates.Visible;
				}


				rated_shows_entry.Click += (sender, args) => onRatedShowsEntryClick(show);
			}


			_main_activity.setToolbarTitle("Rated");
			_main_activity.setIsLoading(false);
			return layout;
		}




		private void onRatedShowsEntryClick(RatedShowSerialized show) {
			_main_activity.setIsLoading(true);
			moveToShowDetailsFrag(show);
		}




		private void moveToShowDetailsFrag(RatedShowSerialized show) {
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