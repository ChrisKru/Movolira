using Android.OS;
using Android.Support.V4.App;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using Av4 = Android.Support.V4.App;
using Android.Support.V7.Widget;
using Newtonsoft.Json;

namespace Movolira{
    public class CardMovieFragment : Av4.Fragment {
        public override void OnCreate(Bundle saved_instance_state) {
            base.OnCreate(saved_instance_state);
            //main_activity = (MainActivity)Activity;
        }
        public override void OnAttach(Context activity) {
            main_activity = (MainActivity)activity;
            base.OnAttach(activity);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            View layout = inflater.Inflate(Resource.Layout.card_movie, container, false);
            List<CardMovie> movie_data = main_activity.tmdb.getPopularMovies();
            CardMovieAdapter recycler_adapter = new CardMovieAdapter(movie_data, main_activity);
            recycler_adapter.click_handler += OnItemClick;
            RecyclerView recycler_view = layout.FindViewById<RecyclerView>(Resource.Id.main_activity_layout);
            recycler_view.SetAdapter(recycler_adapter);
            CardMovieDecoration item_decoration = new CardMovieDecoration(main_activity);
            recycler_view.AddItemDecoration(item_decoration);
            return layout;
        }
        private void OnItemClick(object sender, int position) {
            main_activity.SupportFragmentManager.BeginTransaction().Replace(Resource.Id.main_activity_frame, new MovieDetailsFragment())
                .AddToBackStack(null).Commit();
        }
        private MainActivity main_activity;
    }
}