using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Util;
using Newtonsoft.Json;

namespace Movolira {
	public class ShowListFragment : Fragment, IBackButtonHandler {
		private MainActivity _main_activity;
		private View _frag_layout;
		private RecyclerView _cards_view;
		private ShowCardViewAdapter _cards_view_adapter;
		private ShowCardPreloadModelProvider _preload_model_provider;
		private List<Show> _shows;
		private int _current_page_number = 1;
		private int _max_item_count;

		public override void OnCreate(Bundle saved_instance_state) {
			base.OnCreate(saved_instance_state);
		}

		public override void OnAttach(Context activity) {
			_main_activity = (MainActivity) activity;
			_shows = new List<Show>();
			base.OnAttach(activity);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle saved_instance_state) {
			string type = Arguments.GetString("type");
			string subtype = Arguments.GetString("subtype");
			_main_activity.setToolbarTitle(type, subtype);
			_frag_layout = inflater.Inflate(Resource.Layout.show_list, container, false);
			_cards_view_adapter = new ShowCardViewAdapter(_shows, _main_activity);
			if (_shows.Count == 0) {
				Task.Run(() => fillAdapter(_current_page_number));
			} else {
				_main_activity.setIsLoading(false);
			}
			_cards_view_adapter.ShowCardClickEvent += OnShowCardClick;
			_cards_view_adapter.NextButtonClickEvent += OnNextButtonClick;
			_cards_view_adapter.PrevButtonClickEvent += OnPrevButtonClick;
			_cards_view_adapter.CurrentPageNumber = _current_page_number;
			_cards_view_adapter.MaxItemCount = _max_item_count;
			_cards_view = _frag_layout.FindViewById<RecyclerView>(Resource.Id.show_list_content_layout);
			int display_dpi = (int) _main_activity.Resources.DisplayMetrics.DensityDpi;
			float display_width_pixels = _main_activity.Resources.DisplayMetrics.WidthPixels;
			int span_count = (int) Math.Floor(display_width_pixels / display_dpi / 1.2);
			((GridLayoutManager) _cards_view.GetLayoutManager()).SpanCount = span_count;
			((GridLayoutManager) _cards_view.GetLayoutManager()).SetSpanSizeLookup(new ShowListSpanSizeLookup(span_count, _cards_view_adapter));
			_cards_view.SetAdapter(_cards_view_adapter);
			ShowCardViewDecoration cards_decoration = new ShowCardViewDecoration(_main_activity);
			_cards_view.AddItemDecoration(cards_decoration);
			ViewPreloadSizeProvider preload_size_provider = new ViewPreloadSizeProvider();
			_preload_model_provider = new ShowCardPreloadModelProvider(_shows, _main_activity);
			var cards_view_preloader = new RecyclerViewPreloader<Movie>(_main_activity, _preload_model_provider,
				preload_size_provider, span_count * 3);
			_cards_view.AddOnScrollListener(cards_view_preloader);
			return _frag_layout;
		}

		private async void fillAdapter(int new_page_number) {
			string type = Arguments.GetString("type");
			string subtype = Arguments.GetString("subtype");
			Tuple<List<Show>, int> show_data = null;
			if (type == "movies") {
				if (subtype == "trending") {
					show_data = await _main_activity.DataProvider.getTrendingMovies(new_page_number);
				} else if (subtype == "most_popular") {
					show_data = await _main_activity.DataProvider.getMostPopularMovies(new_page_number);
				} else if (subtype == "most_watched") {
					show_data = await _main_activity.DataProvider.getMostWatchedMovies(new_page_number);
				} else if (subtype == "most_collected") {
					show_data = await _main_activity.DataProvider.getMostCollectedMovies(new_page_number);
				} else if (subtype == "most_anticipated") {
					show_data = await _main_activity.DataProvider.getMostAnticipatedMovies(new_page_number);
				} else if (subtype == "box_office") {
					show_data = await _main_activity.DataProvider.getBoxOfficeMovies();
				}
			} else if (type == "tv_shows") {
				if (subtype == "trending") {
					show_data = await _main_activity.DataProvider.getTrendingTvShows(new_page_number);
				} else if (subtype == "most_popular") {
					show_data = await _main_activity.DataProvider.getMostPopularTvShows(new_page_number);
				} else if (subtype == "most_watched") {
					show_data = await _main_activity.DataProvider.getMostWatchedTvShows(new_page_number);
				} else if (subtype == "most_collected") {
					show_data = await _main_activity.DataProvider.getMostCollectedTvShows(new_page_number);
				} else if (subtype == "most_anticipated") {
					show_data = await _main_activity.DataProvider.getMostAnticipatedTvShows(new_page_number);
				}
			} else if (type == "search") {
				show_data = await _main_activity.DataProvider.searchShows(new_page_number, subtype);
			}

			if (show_data == null) {
				_main_activity.RunOnUiThread(() => {
					_main_activity.setIsLoading(false);
					_main_activity.showNetworkError();
				});
				return;
			}
			var new_shows = show_data.Item1;
			_max_item_count = show_data.Item2;
			_shows = new_shows;
			_current_page_number = new_page_number;
			_preload_model_provider.Shows = new_shows;
			_main_activity.RunOnUiThread(() => {
				_cards_view_adapter.Shows = new_shows;
				_cards_view_adapter.CurrentPageNumber = new_page_number;
				_cards_view_adapter.MaxItemCount = _max_item_count;
				_cards_view_adapter.NotifyDataSetChanged();
				((GridLayoutManager) _cards_view.GetLayoutManager()).ScrollToPositionWithOffset(0, 0);
				_main_activity.setIsLoading(false);
				if (_shows.Count == 0) {
					TextView no_results_text = _frag_layout.FindViewById<TextView>(Resource.Id.show_list_no_shows_text);
					no_results_text.Visibility = ViewStates.Visible;
				}
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
			int new_page_number = _current_page_number + 1;
			Task.Run(() => fillAdapter(new_page_number));
		}

		private void OnPrevButtonClick(object sender, EventArgs args) {
			if (_main_activity.IsLoading) {
				return;
			}
			_main_activity.setIsLoading(true);
			int new_page_number = _current_page_number - 1;
			Task.Run(() => fillAdapter(new_page_number));
		}

		public bool handleBackButtonPress() {
			if (_current_page_number > 1) {
				OnPrevButtonClick(null, null);
				return true;
			}
			return false;
		}

		private void moveToShowDetailsFrag(int position) {
			Fragment details_fragment;
			Bundle fragment_args = new Bundle();
			if (_shows[position].Type == ShowType.Movie) {
				details_fragment = new MovieDetailsFragment();
				Movie movie = _shows[position] as Movie;
				fragment_args.PutString("movie", JsonConvert.SerializeObject(movie));
			} else {
				details_fragment = new TvShowDetailsFragment();
				TvShow tv_show = _shows[position] as TvShow;
				fragment_args.PutString("tv_show", JsonConvert.SerializeObject(tv_show));
			}
			details_fragment.Arguments = fragment_args;
			_main_activity.SupportFragmentManager.BeginTransaction().Replace(Resource.Id.main_activity_fragment_frame, details_fragment)
				.SetTransition(FragmentTransaction.TransitFragmentFade).AddToBackStack(null).Commit();
		}
	}
}