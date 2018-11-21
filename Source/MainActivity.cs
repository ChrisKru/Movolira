using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using System.Collections.Generic;

/*
 * Add support for landscape, tablets
 * Improve layouts on small screens
 * Add handling for server failures
 * TMDB cache, global/service
 */
namespace Movolira {
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : FragmentActivity{
        protected override void OnCreate(Bundle savedInstanceState){
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main_activity);
            tmdb = new TMDBController();
            SupportFragmentManager.BeginTransaction().Add(Resource.Id.main_activity_frame, new CardMovieFragment()).Commit();
        }
        public override void OnBackPressed() {
            if(SupportFragmentManager.BackStackEntryCount > 0) {
                SupportFragmentManager.PopBackStack();
            } else {
                base.OnBackPressed();
            }
        }
        public TMDBController tmdb { get; private set; }
    }
}

