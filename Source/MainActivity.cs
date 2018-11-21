using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using System.Collections.Generic;

/*
 * Add support for landscape, tablets
 * Improve layouts on small screens
 */
namespace Movolira {
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Activity{
        protected override void OnCreate(Bundle savedInstanceState){
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main_activity);
            tmdb = new TMDBController();
            pagePopularMovies();
        }
        private void pagePopularMovies() {
            List<CardMovie> movie_data = tmdb.getPopularMovies();
            CardMovieAdapter adapter = new CardMovieAdapter(movie_data, this);
            RecyclerView layout = FindViewById<RecyclerView>(Resource.Id.main_activity_layout);
            layout.SetAdapter(adapter);
            CardMovieDecoration decoration = new CardMovieDecoration(this);
            layout.AddItemDecoration(decoration);
        }
        TMDBController tmdb;
    }
}

