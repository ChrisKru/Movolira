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
 * Layout for tablet/small
 * Server fail handling
 * Default backdrop backdrops/posters
 * Page scrolling
 * Loading animation
 * Asyncloading
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
            if(saved_state != null) {
                tmdb = JsonConvert.DeserializeObject<TMDBController>(saved_state.GetString("tmdb"));
            } else {
                tmdb = new TMDBController();
            }
            SetContentView(Resource.Layout.main_activity);
            if (SupportFragmentManager.FindFragmentById(Resource.Id.main_activity_frame) == null) {
                SupportFragmentManager.BeginTransaction().Add(Resource.Id.main_activity_frame, new CardMovieFragment(), null).Commit();
            }
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

