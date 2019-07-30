using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Util;
using Newtonsoft.Json;

namespace Movolira {
	public class ShowListFragment : Fragment, IBackButtonHandler {
		private RecyclerView _cards_view;
		private ShowCardViewAdapter _cards_view_adapter;
		private ShowCardPreloadModelProvider _cards_view_preload_model_provider;
		private int _current_page = 1;
		private View _frag_layout;
		private MainActivity _main_activity;
		private List<Movie> _shows;

		public override void OnCreate(Bundle saved_instance_state) {
			base.OnCreate(saved_instance_state);
		}

		public override void OnAttach(Context activity) {
			_main_activity = (MainActivity) activity;
			_shows = new List<Movie>();
			base.OnAttach(activity);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle saved_instance_state) {
			_frag_layout = inflater.Inflate(Resource.Layout.show_list, container, false);
			_cards_view_adapter = new ShowCardViewAdapter(_shows, _main_activity);
			if (_shows.Count == 0) {
				Task.Run(() => fillAdapter());
			} else {
				_main_activity.setIsLoading(false);
			}
			_cards_view_adapter.ShowCardClickEvent += OnShowCardClick;
			_cards_view_adapter.NextButtonClickEvent += OnNextButtonClick;
			_cards_view_adapter.PrevButtonClickEvent += OnPrevButtonClick;
			_cards_view_adapter.CurrentPage = _current_page;
			_cards_view = _frag_layout.FindViewById<RecyclerView>(Resource.Id.show_list_content_layout);
			int display_dpi = (int) _main_activity.Resources.DisplayMetrics.DensityDpi;
			float display_width_pixels = _main_activity.Resources.DisplayMetrics.WidthPixels;
			int span_count = (int) Math.Floor(display_width_pixels / display_dpi / 1.2);
			((GridLayoutManager) _cards_view.GetLayoutManager()).SpanCount = span_count;
			_cards_view.SetAdapter(_cards_view_adapter);
			ShowCardViewDecoration cards_decoration = new ShowCardViewDecoration(_main_activity);
			_cards_view.AddItemDecoration(cards_decoration);
			ViewPreloadSizeProvider preload_size_provider = new ViewPreloadSizeProvider();
			_cards_view_preload_model_provider = new ShowCardPreloadModelProvider(_shows, _main_activity);
			var cards_view_preloader = new RecyclerViewPreloader<Movie>(_main_activity, _cards_view_preload_model_provider,
				preload_size_provider, span_count * 3);
			_cards_view.AddOnScrollListener(cards_view_preloader);
			return _frag_layout;
		}

		private async void fillAdapter() {
			string subtype = Arguments.GetString("subtype");
			if (subtype == "trending") {
				_shows = await _main_activity.DataProvider.getTrendingMovies(_current_page);
			} else if (subtype == "most_popular") {
				_shows = await _main_activity.DataProvider.getMostPopularMovies(_current_page);
			} else if (subtype == "most_watched") {
				_shows = await _main_activity.DataProvider.getMostWatchedMovies(_current_page);
			} else if (subtype == "most_collected") {
				_shows = await _main_activity.DataProvider.getMostCollectedMovies(_current_page);
			} else if (subtype == "most_anticipated") {
				_shows = await _main_activity.DataProvider.getMostAnticipatedMovies(_current_page);
			} else if (subtype == "box_office") {
				_shows = await _main_activity.DataProvider.getBoxOfficeMovies(_current_page);
			}
			if (_shows == null) {
				return;
			}
			_cards_view_preload_model_provider.Shows = _shows;
			_main_activity.RunOnUiThread(() => {
				_cards_view_adapter.Shows = _shows;
				_cards_view_adapter.CurrentPage = _current_page;
				_cards_view_adapter.NotifyDataSetChanged();
				((GridLayoutManager) _cards_view.GetLayoutManager()).ScrollToPositionWithOffset(0, 0);
				_main_activity.setIsLoading(false);
			});
		}

		private void OnShowCardClick(object sender, int position) {
			if (_main_activity.IsLoading) {
				return;
			}
			_main_activity.setIsLoading(true);
			Task.Run(() => moveToShowDetailsFrag(position));
		}

		private void OnNextButtonClick(object sender, EventArgs args) {
			if (_main_activity.IsLoading) {
				return;
			}
			_main_activity.setIsLoading(true);
			_current_page += 1;
			Task.Run(() => fillAdapter());
		}

		private void OnPrevButtonClick(object sender, EventArgs args) {
			if (_main_activity.IsLoading) {
				return;
			}
			_main_activity.setIsLoading(true);
			_current_page -= 1;
			Task.Run(() => fillAdapter());
		}

		public bool handleBackButtonPress() {
			if (_current_page > 1) {
				OnPrevButtonClick(null, null);
				return true;
			}
			return false;
		}

		private void moveToShowDetailsFrag(int position) {
			MovieDetailsFragment details_fragment = new MovieDetailsFragment();
			Bundle frag_args = new Bundle();
			frag_args.PutString("movie", JsonConvert.SerializeObject(_shows[position]));
			details_fragment.Arguments = frag_args;
			_main_activity.SupportFragmentManager.BeginTransaction().Replace(Resource.Id.main_activity_fragment_frame, details_fragment)
				.SetTransition(FragmentTransaction.TransitFragmentFade).AddToBackStack(null).Commit();
		}
	}
}