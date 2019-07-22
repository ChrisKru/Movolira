﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace Movolira {
	public class MovieListFragment : Fragment {
		private RecyclerView _cards_view;
		private MovieCardViewAdapter _cards_view_adapter;
		private int _current_page = 1;
		private View _frag_layout;
		private bool _is_loading;
		private ImageView _loading_view;
		private MainActivity _main_activity;
		private List<Movie> _movies;

		public override void OnCreate(Bundle saved_instance_state) {
			base.OnCreate(saved_instance_state);
		}

		public override void OnAttach(Context activity) {
			_main_activity = (MainActivity) activity;
			_movies = new List<Movie>();
			base.OnAttach(activity);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle saved_instance_state) {
			_frag_layout = inflater.Inflate(Resource.Layout.movie_list, container, false);
			_loading_view = _frag_layout.FindViewById<ImageView>(Resource.Id.movie_list_loading);
			_cards_view_adapter = new MovieCardViewAdapter(_movies, _main_activity);
			_cards_view_adapter.MovieCardClickEvent += OnMovieCardClick;
			_cards_view_adapter.NextButtonClickEvent += OnNextButtonClick;
			_cards_view_adapter.PrevButtonClickEvent += OnPrevButtonClick;
			if (_movies.Count == 0) {
				_loading_view.Visibility = ViewStates.Visible;
				((AnimationDrawable) _loading_view.Background).Start();
				Task.Run(() => fillAdapter());
			}
			_cards_view = _frag_layout.FindViewById<RecyclerView>(Resource.Id.movie_list_content_layout);
			int display_dpi = (int) _main_activity.Resources.DisplayMetrics.DensityDpi;
			float display_width_pixels = _main_activity.Resources.DisplayMetrics.WidthPixels;
			int span_count = (int) Math.Floor(display_width_pixels / display_dpi / 1.2);
			((GridLayoutManager) _cards_view.GetLayoutManager()).SpanCount = span_count;
			_cards_view.SetAdapter(_cards_view_adapter);
			MovieCardViewDecoration cards_decoration = new MovieCardViewDecoration(_main_activity);
			_cards_view.AddItemDecoration(cards_decoration);
			return _frag_layout;
		}

		private void fillAdapter() {
			_movies = _main_activity.MovieDataProvider.getPopularMovies(_current_page);
			_cards_view_adapter.Movies = _movies;
			_main_activity.RunOnUiThread(() => {
				_cards_view_adapter.NotifyDataSetChanged();
				((GridLayoutManager) _cards_view.GetLayoutManager()).ScrollToPositionWithOffset(0, 0);
				_loading_view.Visibility = ViewStates.Gone;
			});
			_is_loading = false;
		}

		private void OnMovieCardClick(object sender, int position) {
			_loading_view.Visibility = ViewStates.Visible;
			((AnimationDrawable) _loading_view.Background).Start();
			Task.Run(() => moveToMovieDetailsFrag(position));
		}

		private void OnNextButtonClick(object sender, EventArgs args) {
			if (_is_loading) {
				return;
			}
			_current_page += 1;
			_loading_view.Visibility = ViewStates.Visible;
			((AnimationDrawable) _loading_view.Background).Start();
			Task.Run(() => fillAdapter());
			_is_loading = true;
		}

		private void OnPrevButtonClick(object sender, EventArgs args) {
			if (_is_loading ) {
				return;
			}
			_current_page -= 1;
			_loading_view.Visibility = ViewStates.Visible;
			((AnimationDrawable) _loading_view.Background).Start();
			Task.Run(() => fillAdapter());
			_is_loading = true;
		}

		private void moveToMovieDetailsFrag(int position) {
			if (_is_loading) {
				return;
			}
			_is_loading = true;
			MovieDetailsFragment details_fragment = new MovieDetailsFragment();
			Bundle frag_args = new Bundle();
			frag_args.PutString("movie", JsonConvert.SerializeObject(_movies[position]));
			details_fragment.Arguments = frag_args;
			_main_activity.SupportFragmentManager.BeginTransaction().Replace(Resource.Id.main_activity_frame, details_fragment)
				.SetTransition(FragmentTransaction.TransitFragmentFade).AddToBackStack(null).Commit();
			_is_loading = false;
		}
	}
}