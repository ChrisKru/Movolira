using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Av4 = Android.Support.V4;

namespace Movolira {
	public class MovieListFragment : Fragment {
		private ImageView _loading;
		private MainActivity _main_activity;
		private List<MovieCard> _movie_data;

		public override void OnCreate(Bundle saved_instance_state) {
			base.OnCreate(saved_instance_state);
		}

		public override void OnAttach(Context activity) {
			_main_activity = (MainActivity) activity;
			base.OnAttach(activity);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			View layout = inflater.Inflate(Resource.Layout.card_movie, container, false);
			_loading = layout.FindViewById<ImageView>(Resource.Id.card_movie_loading);
			_movie_data = _main_activity.MovieDataProvider.getPopularMovies();
			MovieCardViewAdapter recycler_adapter = new MovieCardViewAdapter(_movie_data, _main_activity);
			recycler_adapter.click_handler += OnItemClick;
			RecyclerView recycler_view = layout.FindViewById<RecyclerView>(Resource.Id.card_movie_layout);
			recycler_view.SetAdapter(recycler_adapter);
			MovieCardViewDecoration item_decoration = new MovieCardViewDecoration(_main_activity);
			recycler_view.AddItemDecoration(item_decoration);
			return layout;
		}

		private void OnItemClick(object sender, int position) {
			_loading.Visibility = ViewStates.Visible;
			((AnimationDrawable) _loading.Background).Start();
			Task.Run(() => moveToMovieDetails(position));
		}

		private void moveToMovieDetails(int position) {
			int id = _movie_data[position].Id;
			_main_activity.MovieDataProvider.getMovieDetails(id);
			MovieDetailsFragment details_fragment = new MovieDetailsFragment();
			Bundle args = new Bundle();
			args.PutInt("id", id);
			details_fragment.Arguments = args;
			_main_activity.SupportFragmentManager.BeginTransaction().Replace(Resource.Id.main_activity_frame, details_fragment)
				.SetTransition(FragmentTransaction.TransitFragmentFade).AddToBackStack(null).Commit();
		}
	}
}