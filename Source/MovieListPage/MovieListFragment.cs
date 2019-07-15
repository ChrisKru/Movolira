using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Movolira {
	public class MovieListFragment : Fragment {
		private ImageView _loading_view;
		private MainActivity _main_activity;
		private List<MovieCard> _movie_cards;

		public override void OnCreate(Bundle saved_instance_state) {
			base.OnCreate(saved_instance_state);
		}

		public override void OnAttach(Context activity) {
			_main_activity = (MainActivity) activity;
			base.OnAttach(activity);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			View frag_layout = inflater.Inflate(Resource.Layout.card_movie, container, false);
			_loading_view = frag_layout.FindViewById<ImageView>(Resource.Id.card_movie_loading);
			_movie_cards = _main_activity.MovieDataProvider.getPopularMovieCards();
			MovieCardViewAdapter cards_view_adapter = new MovieCardViewAdapter(_movie_cards, _main_activity);
			cards_view_adapter.click_handler += OnItemClick;
			RecyclerView cards_view = frag_layout.FindViewById<RecyclerView>(Resource.Id.card_movie_layout);
			cards_view.SetAdapter(cards_view_adapter);
			MovieCardViewDecoration cards_decoration = new MovieCardViewDecoration(_main_activity);
			cards_view.AddItemDecoration(cards_decoration);
			return frag_layout;
		}

		private void OnItemClick(object sender, int position) {
			_loading_view.Visibility = ViewStates.Visible;
			((AnimationDrawable) _loading_view.Background).Start();
			Task.Run(() => moveToMovieDetailsFrag(position));
		}

		private void moveToMovieDetailsFrag(int position) {
			int id = _movie_cards[position].Id;
			_main_activity.MovieDataProvider.getDetailedMovie(id);
			MovieDetailsFragment details_fragment = new MovieDetailsFragment();
			Bundle frag_args = new Bundle();
			frag_args.PutInt("id", id);
			details_fragment.Arguments = frag_args;
			_main_activity.SupportFragmentManager.BeginTransaction().Replace(Resource.Id.main_activity_frame, details_fragment)
				.SetTransition(FragmentTransaction.TransitFragmentFade).AddToBackStack(null).Commit();
		}
	}
}