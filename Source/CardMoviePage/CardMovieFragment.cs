using Android.OS;
using Android.Support.V4.App;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using Av4 = Android.Support.V4;
using Android.Support.V7.Widget;
using Newtonsoft.Json;
using Android.Graphics.Drawables;
using System.Threading.Tasks;
using System;

namespace Movolira{
    public class CardMovieFragment : Av4.App.Fragment {
        public override void OnCreate(Bundle saved_instance_state) {
            base.OnCreate(saved_instance_state);
        }
        public override void OnAttach(Context activity) {
            main_activity = (MainActivity)activity;
            base.OnAttach(activity);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            View layout = inflater.Inflate(Resource.Layout.card_movie, container, false);
            loading = layout.FindViewById<ImageView>(Resource.Id.card_movie_loading);
            movie_data = main_activity.tmdb.getPopularMovies();
            CardMovieAdapter recycler_adapter = new CardMovieAdapter(movie_data, main_activity);
            recycler_adapter.click_handler += OnItemClick;
            RecyclerView recycler_view = layout.FindViewById<RecyclerView>(Resource.Id.card_movie_layout);
            recycler_view.SetAdapter(recycler_adapter);
            CardMovieDecoration item_decoration = new CardMovieDecoration(main_activity);
            recycler_view.AddItemDecoration(item_decoration);
            return layout;
        }
        private void OnItemClick(object sender, int position) {
            loading.Visibility = ViewStates.Visible;
            ((AnimationDrawable)loading.Background).Start();
            Task.Run(() => moveToMovieDetails(position));
        }
        private void moveToMovieDetails(int position) {
            int id = movie_data[position].id;
            main_activity.tmdb.getMovieDetails(id);
            MovieDetailsFragment details_fragment = new MovieDetailsFragment();
            Bundle args = new Bundle();
            args.PutInt("id", id);
            details_fragment.Arguments = args;
            main_activity.SupportFragmentManager.BeginTransaction().Replace(Resource.Id.main_activity_frame, details_fragment)
                    .SetTransition(Av4.App.FragmentTransaction.TransitFragmentFade).AddToBackStack(null).Commit();
        }
        private List<CardMovie> movie_data;
        private ImageView loading;
        private MainActivity main_activity;
    }
}