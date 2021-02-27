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
using Movolira.Interfaces;
using Movolira.Pages.ShowDetailsPages;
using Newtonsoft.Json;




namespace Movolira.Pages.ShowListPage {
	public class ShowListFragment : Fragment, IBackButtonHandler {
		private RecyclerView _cards_view;
		private ShowCardViewAdapter _cards_view_adapter;
		private int _current_page_number = 1;
		private View _frag_layout;
		private MainActivity _main_activity;
		private int _max_item_count;
		private ShowCardPreloadModelProvider _preload_model_provider;
		private List<Show> _shows;




		public override void OnCreate(Bundle saved_instance_state) {
			base.OnCreate(saved_instance_state);
		}




		public override void OnAttach(Context activity) {
			this._main_activity = (MainActivity)activity;
			this._shows = new List<Show>();
			base.OnAttach(activity);
		}




		public override View OnCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle saved_instance_state) {


			string type = this.Arguments.GetString("type");
			string subtype = this.Arguments.GetString("subtype");
			this._main_activity.setToolbarTitle(type, subtype);
			this._frag_layout = inflater.Inflate(Resource.Layout.show_list_page, container, false);
			this._cards_view_adapter = new ShowCardViewAdapter(this._shows, this._main_activity);


			if (this._shows.Count == 0) {
				Task.Run(() => this.fillAdapter(this._current_page_number));
			} else {
				this._main_activity.setIsLoading(false);
			}


			this._cards_view_adapter.ShowCardClickEvent += this.OnShowCardClick;
			this._cards_view_adapter.NextButtonClickEvent += this.OnNextButtonClick;
			this._cards_view_adapter.PrevButtonClickEvent += this.OnPrevButtonClick;
			this._cards_view_adapter.CurrentPageNumber = this._current_page_number;
			this._cards_view_adapter.MaxItemCount = this._max_item_count;
			this._cards_view = this._frag_layout.FindViewById<RecyclerView>(Resource.Id.show_list_content_layout);


			int display_dpi = (int)this._main_activity.Resources.DisplayMetrics.DensityDpi;
			float display_width_pixels = this._main_activity.Resources.DisplayMetrics.WidthPixels;
			int span_count = (int)Math.Floor(display_width_pixels / display_dpi / 1.1);
			((GridLayoutManager)this._cards_view.GetLayoutManager()).SpanCount = span_count;
			((GridLayoutManager)this._cards_view.GetLayoutManager())
				.SetSpanSizeLookup(new ShowListSpanSizeLookup(span_count, this._cards_view_adapter));
			this._cards_view.SetAdapter(this._cards_view_adapter);
			ShowCardViewDecoration cards_decoration = new ShowCardViewDecoration(this._main_activity);
			this._cards_view.AddItemDecoration(cards_decoration);


			ViewPreloadSizeProvider preload_size_provider = new ViewPreloadSizeProvider();
			this._preload_model_provider = new ShowCardPreloadModelProvider(this._shows, this._main_activity);
			var cards_view_preloader = new RecyclerViewPreloader<Movie>(this._main_activity, this._preload_model_provider,
				preload_size_provider, span_count * 3);
			this._cards_view.AddOnScrollListener(cards_view_preloader);


			return this._frag_layout;
		}




		private async void fillAdapter(int new_page_number) {
			var show_data = await this.getShowData(new_page_number);
			if (show_data == null) {
				this._main_activity.RunOnUiThread(() => {
					this._main_activity.setIsLoading(false);
					this._main_activity.showNetworkError();
				});
				return;
			}


			var new_shows = show_data.Item1;
			this._max_item_count = show_data.Item2;
			this._shows = new_shows;
			this._current_page_number = new_page_number;
			this._preload_model_provider.Shows = new_shows;


			this._main_activity.RunOnUiThread(() => {
				this._cards_view_adapter.Shows = new_shows;
				this._cards_view_adapter.CurrentPageNumber = new_page_number;
				this._cards_view_adapter.MaxItemCount = this._max_item_count;
				this._cards_view_adapter.NotifyDataSetChanged();
				((GridLayoutManager)this._cards_view.GetLayoutManager()).ScrollToPositionWithOffset(0, 0);
				this._main_activity.setIsLoading(false);


				if (this._shows.Count == 0) {
					TextView no_results_text = this._frag_layout.FindViewById<TextView>(Resource.Id.show_list_no_shows_text);
					no_results_text.Visibility = ViewStates.Visible;
				} else {
					TextView no_results_text = this._frag_layout.FindViewById<TextView>(Resource.Id.show_list_no_shows_text);
					no_results_text.Visibility = ViewStates.Gone;
				}
			});
		}




		private async Task<Tuple<List<Show>, int>> getShowData(int new_page_number) {
			string type = this.Arguments.GetString("type");
			string subtype = this.Arguments.GetString("subtype");
			Tuple<List<Show>, int> show_data = null;


			if (type == "movies") {
				show_data = await this._main_activity.MovieProvider.getMovies(subtype, new_page_number);
			} else if (type == "tv_shows") {
				show_data = await this._main_activity.TvShowProvider.getTvShows(subtype, new_page_number);
			} else if (type == "search") {
				show_data = await this._main_activity.ShowProvider.getSearchedShows(subtype, new_page_number);
			} else if (type == "discover") {
				show_data = await this._main_activity.ShowProvider.getDiscoveredShows(subtype, new_page_number);
			}


			return show_data;
		}




		private void OnShowCardClick(object sender, int position) {
			if (this._main_activity.IsLoading) {
				return;
			}
			this._main_activity.setIsLoading(true);
			Task.Run(() => this.moveToShowDetailsFrag(position));
		}




		private void OnNextButtonClick(object sender, EventArgs args) {
			if (this._main_activity.IsLoading) {
				return;
			}
			this._main_activity.setIsLoading(true);
			int new_page_number = this._current_page_number + 1;
			Task.Run(() => this.fillAdapter(new_page_number));
		}




		private void OnPrevButtonClick(object sender, EventArgs args) {
			if (this._main_activity.IsLoading) {
				return;
			}
			this._main_activity.setIsLoading(true);
			int new_page_number = this._current_page_number - 1;
			Task.Run(() => this.fillAdapter(new_page_number));
		}




		public bool handleBackButtonPress() {
			if (this._current_page_number > 1) {
				this.OnPrevButtonClick(null, null);
				return true;
			}
			return false;
		}




		private void moveToShowDetailsFrag(int position) {
			Fragment details_fragment;
			Bundle fragment_args = new Bundle();


			if (this._shows[position].Type == ShowType.Movie) {
				details_fragment = new MovieDetailsFragment();
				Movie movie = this._shows[position] as Movie;
				fragment_args.PutString("movie", JsonConvert.SerializeObject(movie));
			} else {
				details_fragment = new TvShowDetailsFragment();
				TvShow tv_show = this._shows[position] as TvShow;
				fragment_args.PutString("tv_show", JsonConvert.SerializeObject(tv_show));
			}


			details_fragment.Arguments = fragment_args;
			this._main_activity.SupportFragmentManager.BeginTransaction()
				.Replace(Resource.Id.main_activity_fragment_frame, details_fragment)
				.SetTransition(FragmentTransaction.TransitFragmentFade).AddToBackStack(null).Commit();
		}
	}
}