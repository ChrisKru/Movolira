using System.Globalization;
using System.Threading.Tasks;
using Android.App;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using Movolira.DataProviders;
using Movolira.Pages.DiscoverPage;
using Movolira.Pages.RatedShowsPage;
using Movolira.Pages.SettingsPage;
using Movolira.Pages.ShowDetailsPages;
using Movolira.Pages.ShowListPage;
using Movolira.Pages.WatchlistPage;
using Newtonsoft.Json;
using Fragment = AndroidX.Fragment.App.Fragment;
using FragmentTransaction = AndroidX.Fragment.App.FragmentTransaction;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;




namespace Movolira {
	[Activity(Label = "@string/app_name", Theme = "@style/AppThemeBlue", MainLauncher = true,
		WindowSoftInputMode = SoftInput.StateUnchanged | SoftInput.AdjustResize)]
	public class MainActivity : AppCompatActivity {
		public static readonly int SHOWS_PER_PAGE = 20;


		public GenresProvider GenresProvider { get; private set; }
		public MovieProvider MovieProvider { get; private set; }
		public TvShowProvider TvShowProvider { get; private set; }
		public ShowProvider ShowProvider { get; private set; }
		public SettingsProvider SettingsProvider { get; private set; }
		public bool IsLoading { get; private set; }


		private DrawerLayout _drawer_layout;
		private ActionBarDrawerToggle _drawer_toggle;
		// Counts how many methods are currently loading data.
		// As long as at least 1 method is loading data, the "loading" animation is shown.
		private int _loading_count;
		private ImageView _loading_view;
		private Toolbar _toolbar;




		protected override void OnCreate(Bundle saved_instance_state) {
			base.OnCreate(saved_instance_state);
			this.constructProviders();
			this.SetContentView(Resource.Layout.main_activity);
			MainActivityViewBuilder.buildLoadingView(this, out this._loading_view);
			MainActivityViewBuilder.buildToolbar(this, saved_instance_state, out this._toolbar);
			MainActivityViewBuilder.buildDrawerMenu(this, this._toolbar, out this._drawer_layout, out this._drawer_toggle);


			// If the activity was restarted by the settings page fragment, it goes directly to it.
			if (this.Intent.GetBooleanExtra("was_restarted_by_settings_page", false)) {
				SettingsFragment settings_frag = new SettingsFragment();
				Bundle fragment_args = new Bundle();
				fragment_args.PutString("fragment_type", "settings");
				settings_frag.Arguments = fragment_args;
				this.SupportFragmentManager.BeginTransaction()
					.Add(Resource.Id.main_activity_fragment_frame, settings_frag, null).Commit();


			// If no fragments have been constructed yet, the default fragment is created,
			// which is "movies/popular" listings page.
			} else if (this.SupportFragmentManager.FindFragmentById(Resource.Id.main_activity_fragment_frame) == null) {
				ShowListFragment content_frag = new ShowListFragment();
				Bundle fragment_args = new Bundle();
				fragment_args.PutString("fragment_type", "movies");
				fragment_args.PutString("fragment_subtype", "popular");
				content_frag.Arguments = fragment_args;
				this.SupportFragmentManager.BeginTransaction()
					.Add(Resource.Id.main_activity_fragment_frame, content_frag, null).Commit();
			}
		}




		private void constructProviders() {
			this.GenresProvider = new GenresProvider();
			this.MovieProvider = new MovieProvider(this.GenresProvider);
			this.TvShowProvider = new TvShowProvider(this.GenresProvider);
			this.ShowProvider = new ShowProvider(this.GenresProvider, this.MovieProvider, this.TvShowProvider);
			this.SettingsProvider = new SettingsProvider(this);
		}




		public override bool OnCreateOptionsMenu(IMenu menu) {
			this.MenuInflater.Inflate(Resource.Menu.main_activity_toolbar_menu, menu);
			MainActivityViewBuilder.buildSearchView(this, menu);
			return true;
		}




		// Synchronizes drawer menu's state with the layout.
		protected override void OnPostCreate(Bundle saved_instance_state) {
			this._drawer_toggle.SyncState();
			base.OnPostCreate(saved_instance_state);
		}




		protected override void OnSaveInstanceState(Bundle saved_instance_state) {
			saved_instance_state.PutString("toolbar_title", this._toolbar.Title);
			base.OnSaveInstanceState(saved_instance_state);
		}




		public void setIsLoading(bool is_loading) {
			this.IsLoading = is_loading;
			if (is_loading) {
				++this._loading_count;
				this._loading_view.Visibility = ViewStates.Visible;


			} else {
				if (this._loading_count > 0) {
					--this._loading_count;
				}


				if (this._loading_count == 0) {
					// A short delay is added to prevent weird "blinking" entry/re-entry of the loading animation view.
					Task.Delay(200).ContinueWith(
						a => this.RunOnUiThread(() => this._loading_view.Visibility = ViewStates.Gone)
					);
				}
			}
		}




		public void showNetworkError() {
			ConnectivityManager network_manager = this.GetSystemService(ConnectivityService) as ConnectivityManager;
			if (network_manager.ActiveNetwork == null) {
				Toast.MakeText(this, "No internet connection", ToastLength.Short).Show();
			} else {
				Toast.MakeText(this, "Server error", ToastLength.Short).Show();
			}
		}




		public void setToolbarTitle(string title) {
			this._toolbar.Title = title;
		}




		// The fragment types/subtypes are used in querying the show data.
		// This method makes some changes to the type/subtype strings to present them in the form of a title.
		public void setToolbarTitle(string fragment_type, string fragment_subtype) {
			TextInfo text_info = new CultureInfo("en-US", false).TextInfo;
			string title_a = fragment_type.Replace("_", " ");
			title_a = text_info.ToTitleCase(title_a);


			string title_b = fragment_subtype;
			if (fragment_type != "search") {
				title_b = fragment_subtype.Replace("_", " ");
				title_b = text_info.ToTitleCase(title_b);
			}


			if (fragment_type == "discover" || fragment_type == "watchlist") {
				this._toolbar.Title = title_a;
			} else {
				this._toolbar.Title = title_a + ": " + title_b;
			}
		}




		public void changeContentFragment(string fragment_type, string fragment_subtype) {
			this.RunOnUiThread(() => { this.setIsLoading(true); });
			Bundle fragment_args = new Bundle();
			fragment_args.PutString("fragment_type", fragment_type);
			fragment_args.PutString("fragment_subtype", fragment_subtype);


			Fragment content_fragment;
			if (fragment_type == "movies" || fragment_type == "tv_shows"
				|| fragment_type == "search" || fragment_type == "discover" && fragment_subtype != "") {

				content_fragment = new ShowListFragment();
			} else if (fragment_type == "discover" && fragment_subtype == "") {
				content_fragment = new DiscoverFragment();
			} else if (fragment_type == "watchlist") {
				content_fragment = new WatchlistFragment();
			} else if (fragment_type == "rated_shows") {
				content_fragment = new RatedShowsFragment();
			} else {
				content_fragment = new SettingsFragment();
			}
			content_fragment.Arguments = fragment_args;


			this.SupportFragmentManager.BeginTransaction().Replace(Resource.Id.main_activity_fragment_frame, content_fragment)
				.SetTransition(FragmentTransaction.TransitFragmentFade).AddToBackStack(null).Commit();
		}




		public void submitSearch(string query) {
			this.changeContentFragment("search", query);
		}




		public void moveToRecommendationFragment() {
			this.RunOnUiThread(() => this.setIsLoading(true));
			var recommendation_show_pair = Recommendation.getRecommendation(this);
			Show recommended_show = recommendation_show_pair.Item1;
			string recommended_show_type = recommendation_show_pair.Item2;
			Fragment details_fragment;
			Bundle fragment_args = new Bundle();


			if (recommended_show_type == "movie") {
				details_fragment = new MovieDetailsFragment();
				Movie movie = recommended_show as Movie;
				fragment_args.PutString("movie", JsonConvert.SerializeObject(movie));
				details_fragment.Arguments = fragment_args;


			} else {
				details_fragment = new TvShowDetailsFragment();
				TvShow movie = recommended_show as TvShow;
				fragment_args.PutString("tv_show", JsonConvert.SerializeObject(movie));
				details_fragment.Arguments = fragment_args;
			}


			this.SupportFragmentManager.BeginTransaction()
				.Replace(Resource.Id.main_activity_fragment_frame, details_fragment)
				.SetTransition(FragmentTransaction.TransitFragmentFade).AddToBackStack(null).Commit();
		}




		public override void OnBackPressed() {
			if (this._drawer_layout.IsDrawerOpen(GravityCompat.Start)) {
				this._drawer_layout.CloseDrawer(GravityCompat.Start);
				return;
			}


			// Calls the back button handle method of the currently active fragment.
			var fragments = this.SupportFragmentManager.Fragments;
			for (int i_fragment = 0; i_fragment < fragments.Count; ++i_fragment) {
				if (this.SupportFragmentManager.Fragments[i_fragment] is IBackButtonHandler back_button_handler) {
					if (back_button_handler.handleBackButtonPress()) {
						return;
					}
				}
			}


			// For the fragments that don't implement the back button handler interface,
			// the default action is to pop the backstack.
			if (this.SupportFragmentManager.BackStackEntryCount > 0) {
				this.clearLoading();
				this.SupportFragmentManager.PopBackStack();
			} else {
				base.OnBackPressed();
			}
		}




		// Hides the loading view regardless of how many methods are currently loading.
		// Used when popping back stack, because the page in the back stack is always completely loaded,
		// (loading view disables user interaction, so the user cannot navigate forwards during loading).
		private void clearLoading() {
			this._loading_count = 0;
			Task.Delay(200).ContinueWith(a => this.RunOnUiThread(
				() => this._loading_view.Visibility = ViewStates.Gone)
			);
		}
	}
}