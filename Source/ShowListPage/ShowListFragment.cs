using System;
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
	public class ShowListFragment : Fragment {
		private RecyclerView _cards_view;
		private ShowCardViewAdapter _cards_view_adapter;
		private int _current_page = 1;
		private View _frag_layout;
		private bool _is_loading;
		private ImageView _loading_view;
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
			_loading_view = _frag_layout.FindViewById<ImageView>(Resource.Id.show_list_loading);
			_cards_view_adapter = new ShowCardViewAdapter(_shows, _main_activity);
			_cards_view_adapter.ShowCardClickEvent += OnShowCardClick;
			_cards_view_adapter.NextButtonClickEvent += OnNextButtonClick;
			_cards_view_adapter.PrevButtonClickEvent += OnPrevButtonClick;
			if (_shows.Count == 0) {
				_loading_view.Visibility = ViewStates.Visible;
				((AnimationDrawable) _loading_view.Background).Start();
				Task.Run(() => fillAdapter());
			}
			_cards_view = _frag_layout.FindViewById<RecyclerView>(Resource.Id.show_list_content_layout);
			int display_dpi = (int) _main_activity.Resources.DisplayMetrics.DensityDpi;
			float display_width_pixels = _main_activity.Resources.DisplayMetrics.WidthPixels;
			int span_count = (int) Math.Floor(display_width_pixels / display_dpi / 1.2);
			((GridLayoutManager) _cards_view.GetLayoutManager()).SpanCount = span_count;
			_cards_view.SetAdapter(_cards_view_adapter);
			ShowCardViewDecoration cards_decoration = new ShowCardViewDecoration(_main_activity);
			_cards_view.AddItemDecoration(cards_decoration);
			return _frag_layout;
		}

		private void fillAdapter() {
			_is_loading = true;
			_shows = _main_activity.ShowDataProvider.getPopularMovies(_current_page);
			_main_activity.RunOnUiThread(() => {
				_cards_view_adapter.Shows = _shows;
				_cards_view_adapter.CurrentPage = _current_page;
				_cards_view_adapter.NotifyDataSetChanged();
				((GridLayoutManager) _cards_view.GetLayoutManager()).ScrollToPositionWithOffset(0, 0);
				_loading_view.Visibility = ViewStates.Gone;
			});
			_is_loading = false;
		}

		private void OnShowCardClick(object sender, int position) {
			if (_is_loading) {
				return;
			}
			_loading_view.Visibility = ViewStates.Visible;
			((AnimationDrawable) _loading_view.Background).Start();
			Task.Run(() => moveToShowDetailsFrag(position));
		}

		private void OnNextButtonClick(object sender, EventArgs args) {
			if (_is_loading) {
				return;
			}
			_current_page += 1;
			_loading_view.Visibility = ViewStates.Visible;
			((AnimationDrawable) _loading_view.Background).Start();
			Task.Run(() => fillAdapter());
		}

		private void OnPrevButtonClick(object sender, EventArgs args) {
			if (_is_loading) {
				return;
			}
			_current_page -= 1;
			_loading_view.Visibility = ViewStates.Visible;
			((AnimationDrawable) _loading_view.Background).Start();
			Task.Run(() => fillAdapter());
		}

		private void moveToShowDetailsFrag(int position) {
			_is_loading = true;
			MovieDetailsFragment details_fragment = new MovieDetailsFragment();
			Bundle frag_args = new Bundle();
			frag_args.PutString("movie", JsonConvert.SerializeObject(_shows[position]));
			details_fragment.Arguments = frag_args;
			_main_activity.SupportFragmentManager.BeginTransaction().Replace(Resource.Id.main_activity_frame, details_fragment)
				.SetTransition(FragmentTransaction.TransitFragmentFade).AddToBackStack(null).Commit();
			_is_loading = false;
		}
	}
}