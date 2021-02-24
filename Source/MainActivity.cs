using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Android.Animation;
using Android.App;
using Android.Graphics.Drawables;
using Android.Net;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Movolira.DataProviders;
using Movolira.Interfaces;
using Movolira.Pages.DiscoverPage;
using Movolira.Pages.RatedShowsPage;
using Movolira.Pages.ShowDetailsPages;
using Movolira.Pages.ShowListPage;
using Movolira.Pages.WatchlistPage;
using Newtonsoft.Json;


using Fragment = Android.Support.V4.App.Fragment;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using Random = System.Random;
using SearchView = Android.Support.V7.Widget.SearchView;
using Toolbar = Android.Support.V7.Widget.Toolbar;




namespace Movolira {
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true,
		WindowSoftInputMode = SoftInput.StateUnchanged | SoftInput.AdjustResize)]
	public class MainActivity : AppCompatActivity {
		public const int SHOWS_PER_PAGE = 20;


		public GenresProvider GenresProvider { get; private set; }
		public MovieProvider MovieProvider { get; private set; }
		public TvShowProvider TvShowProvider { get; private set; }
		public ShowProvider ShowProvider { get; private set; }
		public UserData UserData { get; private set; }
		public bool IsLoading { get; private set; }


		private DrawerLayout _drawer_layout;
		private ActionBarDrawerToggle _drawer_toggle;
		private int _loading_count;
		private ImageView _loading_view;
		private Toolbar _toolbar;




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
					Task.Delay(200).ContinueWith(
						a => this.RunOnUiThread(() => this._loading_view.Visibility = ViewStates.Gone)
					);
				}
			}
		}




		public void showNetworkError() {
			ConnectivityManager network_manager = this.GetSystemService(ConnectivityService) as ConnectivityManager;
			if (network_manager.ActiveNetworkInfo == null) {
				Toast.MakeText(this, "No internet connection", ToastLength.Short).Show();
			} else {
				Toast.MakeText(this, "Server error", ToastLength.Short).Show();
			}
		}




		public void changeContentFragment(string type, string subtype) {
			this.RunOnUiThread(() => { this.setIsLoading(true); });
			Bundle fragment_args = new Bundle();
			fragment_args.PutString("type", type);
			fragment_args.PutString("subtype", subtype);


			Fragment content_fragment;
			if (type == "movies" || type == "tv_shows" || type == "search" || type == "discover" && subtype != "") {
				content_fragment = new ShowListFragment();
			} else if (type == "discover" && subtype == "") {
				content_fragment = new DiscoverFragment();
			} else if (type == "watchlist") {
				content_fragment = new WatchlistFragment();
			} else {
				content_fragment = new RatedShowsFragment();
			}
			content_fragment.Arguments = fragment_args;


			this.SupportFragmentManager.BeginTransaction().Replace(Resource.Id.main_activity_fragment_frame, content_fragment)
				.SetTransition(FragmentTransaction.TransitFragmentFade).AddToBackStack(null).Commit();
		}




		public void getRecommendation() {
			this.RunOnUiThread(() => this.setIsLoading(true));


			HashSet<string> known_show_ids = this.UserData.getKnownShowIds();
			Random randomizer = new Random();
			List<Show> shows = null;
			Show recommended_show = null;
			string recommended_show_type = null;
			int page_count = 9999;
			List<RatedShowSerialized> five_star_shows = this.UserData.getFiveStarShows();


			while (five_star_shows.Any()) {
				int i_show = randomizer.Next(0, five_star_shows.Count - 1);
				string category = five_star_shows[i_show].Id + "/similar";


				for (int i_page = 1; i_page < page_count; ++i_page) {
					Tuple<List<Show>, int> show_data = null;
					if (five_star_shows[i_show].Type == ShowType.Movie.ToString()) {
						show_data = this.MovieProvider.getMovies(category, i_page).Result;
						recommended_show_type = "movie";
					} else {
						show_data = this.TvShowProvider.getTvShows(category, i_page).Result;
						recommended_show_type = "tv_show";
					}


					if (show_data == null) {
						this.RunOnUiThread(() => {
							this.setIsLoading(false);
							this.showNetworkError();
						});
						return;
					}


					shows = show_data.Item1;
					page_count = show_data.Item2 / SHOWS_PER_PAGE;
					if (shows.Any()) {
						for (int i_similar_shows = 0; i_similar_shows < shows.Count; ++i_similar_shows) {
							if (!known_show_ids.Contains(shows[i_similar_shows].Id)) {
								recommended_show = shows[i_similar_shows];
								break;
							}
						}
					}


					if (recommended_show != null) {
						break;
					}
				}


				if (recommended_show != null) {
					break;
				}
				five_star_shows.RemoveAt(i_show);
			}


			if (recommended_show == null) {
				page_count = 9999;
				List<RatedShowSerialized> four_star_shows = this.UserData.getFourStarShows();
				while (four_star_shows.Any()) {
					int i_show = randomizer.Next(0, four_star_shows.Count - 1);
					string category = four_star_shows[i_show].Id + "/similar";


					for (int i_page = 1; i_page < page_count; ++i_page) {
						Tuple<List<Show>, int> show_data = null;
						if (four_star_shows[i_show].Type == ShowType.Movie.ToString()) {
							show_data = this.MovieProvider.getMovies(category, i_page).Result;
							recommended_show_type = "movie";
						} else {
							show_data = this.TvShowProvider.getTvShows(category, i_page).Result;
							recommended_show_type = "tv_show";
						}


						if (show_data == null) {
							this.RunOnUiThread(() => {
								this.setIsLoading(false);
								this.showNetworkError();
							});
							return;
						}


						shows = show_data.Item1;
						page_count = show_data.Item2 / SHOWS_PER_PAGE;
						if (shows.Any()) {
							for (int i_similar_shows = 0; i_similar_shows < shows.Count; ++i_similar_shows) {
								if (!known_show_ids.Contains(shows[i_similar_shows].Id)) {
									recommended_show = shows[i_similar_shows];
									break;
								}
							}
						}


						if (recommended_show != null) {
							break;
						}
					}


					if (recommended_show != null) {
						break;
					}
					four_star_shows.RemoveAt(i_show);
				}
			}


			if (recommended_show == null) {
				page_count = 9999;
				for (int i_page = 1; i_page < page_count; ++i_page) {
					Tuple<List<Show>, int> show_data = null;
					if (randomizer.Next(0, 1) == 0) {
						show_data = this.MovieProvider.getMovies("popular", i_page).Result;
						recommended_show_type = "movie";
					} else {
						show_data = this.TvShowProvider.getTvShows("popular", i_page).Result;
						recommended_show_type = "tv_show";
					}


					if (show_data == null) {
						this.RunOnUiThread(() => {
							this.setIsLoading(false);
							this.showNetworkError();
						});
						return;
					}


					shows = show_data.Item1;
					page_count = show_data.Item2 / SHOWS_PER_PAGE;
					if (shows.Any()) {
						for (int i_similar_shows = 0; i_similar_shows < shows.Count; ++i_similar_shows) {
							if (!known_show_ids.Contains(shows[i_similar_shows].Id)) {
								recommended_show = shows[i_similar_shows];
								break;
							}
						}
					}


					if (recommended_show != null) {
						break;
					}
				}
			}


			if (recommended_show == null) {
				this.RunOnUiThread(() => {
					this.setIsLoading(false);
					this.showNetworkError();
				});
				return;
			}
			this.UserData.addToAlreadyRecommendedShows(recommended_show);
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




		public void submitSearch(string query) {
			this.changeContentFragment("search", query);
		}




		public void setToolbarTitle(string title) {
			this._toolbar.Title = title;
		}




		public void setToolbarTitle(string type, string subtype) {
			TextInfo text_info = new CultureInfo("en-US", false).TextInfo;
			string title_a = type.Replace("_", " ");
			title_a = text_info.ToTitleCase(title_a);


			string title_b = subtype;
			if (type != "search") {
				title_b = subtype.Replace("_", " ");
				title_b = text_info.ToTitleCase(title_b);
			}


			if (type == "discover" || type == "watchlist") {
				this._toolbar.Title = title_a;
			} else {
				this._toolbar.Title = title_a + ": " + title_b;
			}
		}




		protected override void OnCreate(Bundle saved_instance_state) {
			base.OnCreate(saved_instance_state);
			this.UserData = new UserData();
			this.GenresProvider = new GenresProvider();
			this.MovieProvider = new MovieProvider(this.GenresProvider);
			this.TvShowProvider = new TvShowProvider(this.GenresProvider);
			this.ShowProvider = new ShowProvider(this.GenresProvider, this.MovieProvider, this.TvShowProvider);
			this.SetContentView(Resource.Layout.main_activity);
			this._loading_view = this.FindViewById<ImageView>(Resource.Id.show_list_loading);
			((AnimationDrawable)this._loading_view.Background).Start();
			this.setIsLoading(true);


			this._toolbar = this.FindViewById<Toolbar>(Resource.Id.main_activity_toolbar);
			if (saved_instance_state != null) {
				this._toolbar.Title = saved_instance_state.GetString("toolbar_title");
			}
			this.SetSupportActionBar(this._toolbar);
			this.SupportActionBar.SetDisplayHomeAsUpEnabled(true);


			this._drawer_layout = this.FindViewById<DrawerLayout>(Resource.Id.main_activity_drawer_layout);
			this._drawer_toggle = new ActionBarDrawerToggle(this, this._drawer_layout, this._toolbar,
				Android.Resource.Drawable.IcMenuDirections, Android.Resource.Drawable.IcMenuDirections);
			this._drawer_layout.AddDrawerListener(this._drawer_toggle);


			LinearLayout drawer_menu = this.FindViewById<LinearLayout>(Resource.Id.main_activity_navigation_menu);
			LayoutTransition drawer_menu_transition = new LayoutTransition();
			drawer_menu_transition.DisableTransitionType(LayoutTransitionType.Disappearing);
			drawer_menu.LayoutTransition = drawer_menu_transition;
			MenuOnClickListener menu_on_click_listener = new MenuOnClickListener(this, this._drawer_layout);
			for (int i_menu_children = 0; i_menu_children < drawer_menu.ChildCount; ++i_menu_children) {
				drawer_menu.GetChildAt(i_menu_children).SetOnClickListener(menu_on_click_listener);
			}


			if (this.SupportFragmentManager.FindFragmentById(Resource.Id.main_activity_fragment_frame) == null) {
				ShowListFragment content_frag = new ShowListFragment();
				Bundle fragment_args = new Bundle();
				fragment_args.PutString("type", "movies");
				fragment_args.PutString("subtype", "popular");
				content_frag.Arguments = fragment_args;
				this.SupportFragmentManager.BeginTransaction()
					.Add(Resource.Id.main_activity_fragment_frame, content_frag, null).Commit();
			}
		}




		public override bool OnCreateOptionsMenu(IMenu menu) {
			this.MenuInflater.Inflate(Resource.Menu.main_activity_toolbar_menu, menu);
			this.buildSearchView(menu);
			return true;
		}




		private void buildSearchView(IMenu menu) {
			IMenuItem search_item = menu.FindItem(Resource.Id.toolbar_menu_search);
			SearchView search_view = (SearchView)search_item.ActionView;
			search_view.QueryHint = "Movie or Tv show";
			search_view.SetOnQueryTextListener(new SearchQueryTextListener(this, search_item));
		}




		protected override void OnPostCreate(Bundle saved_instance_state) {
			this._drawer_toggle.SyncState();
			base.OnPostCreate(saved_instance_state);
		}




		protected override void OnSaveInstanceState(Bundle saved_instance_state) {
			saved_instance_state.PutString("toolbar_title", this._toolbar.Title);
			base.OnSaveInstanceState(saved_instance_state);
		}




		public override void OnBackPressed() {
			if (this._drawer_layout.IsDrawerOpen(GravityCompat.Start)) {
				this._drawer_layout.CloseDrawer(GravityCompat.Start);
				return;
			}


			var fragments = this.SupportFragmentManager.Fragments;
			for (int i_fragment = 0; i_fragment < fragments.Count; ++i_fragment) {
				if (this.SupportFragmentManager.Fragments[i_fragment] is IBackButtonHandler back_button_handler) {
					if (back_button_handler.handleBackButtonPress()) {
						return;
					}
				}
			}


			if (this.SupportFragmentManager.BackStackEntryCount > 0) {
				this.clearLoading();
				this.SupportFragmentManager.PopBackStack();
				return;
			}
			base.OnBackPressed();
		}




		private void clearLoading() {
			this._loading_count = 0;
			Task.Delay(200).ContinueWith(a => this.RunOnUiThread(
				() => this._loading_view.Visibility = ViewStates.Gone)
			);
		}




		public override bool DispatchTouchEvent(MotionEvent motion_event) {
			var fragments = this.SupportFragmentManager.Fragments;
			for (int i_fragment = 0; i_fragment < fragments.Count; ++i_fragment) {
				if (this.SupportFragmentManager.Fragments[i_fragment] is ITouchHandler back_button_handler) {
					back_button_handler.handleTouch(motion_event);
				}
			}
			return base.DispatchTouchEvent(motion_event);
		}
	}
}