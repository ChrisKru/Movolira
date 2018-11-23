using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Newtonsoft.Json;
using System.Collections.Generic;
/*
 * Add support for landscape, tablets
 * Improve layouts on small screens
 * Add handling for server failures
 */
namespace Movolira {
    [Activity(
        Label = "@string/app_name", 
        Theme = "@style/AppTheme", 
        //ConfigurationChanges = ConfigChanges.Orientation,
        MainLauncher = true)]
    public class MainActivity : FragmentActivity{
        protected override void OnCreate(Bundle saved_state){
            base.OnCreate(saved_state);
            SetContentView(Resource.Layout.main_activity);
            if(saved_state != null) {
                tmdb = JsonConvert.DeserializeObject<TMDBController>(saved_state.GetString("tmdb"));
            } else {
                tmdb = new TMDBController();
            }
            SupportFragmentManager.BeginTransaction().Add(Resource.Id.main_activity_frame, new CardMovieFragment()).Commit();
        }
        public override void OnBackPressed() {
            if(SupportFragmentManager.BackStackEntryCount > 0) {
                SupportFragmentManager.PopBackStack();
            } else {
                base.OnBackPressed();
            }
        }
        protected override void OnSaveInstanceState(Bundle out_state) {
            out_state.PutString("tmdb", JsonConvert.SerializeObject(tmdb));
            base.OnSaveInstanceState(out_state);
        }
        public TMDBController tmdb { get; private set; }
    }
}

