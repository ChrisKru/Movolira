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
using Java.Util;
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
		public DataProvider DataProvider { get; private set; }
		public UserData UserData { get; private set; }
		public bool IsLoading { get; private set; }
		private DrawerLayout _drawer_layout;
		private ActionBarDrawerToggle _drawer_toggle;
		private int _loading_count;
		private ImageView _loading_view;
		private Toolbar _toolbar;




		public void setIsLoading(bool is_loading) {
			IsLoading = is_loading;
			if (is_loading) {
				++_loading_count;
				_loading_view.Visibility = ViewStates.Visible;
			} else {
				if (_loading_count > 0) {
					--_loading_count;
				}
				if (_loading_count == 0) {
					Task.Delay(200).ContinueWith(a => RunOnUiThread(() => _loading_view.Visibility = ViewStates.Gone));
				}
			}
		}




		public void showNetworkError() {
			ConnectivityManager network_manager = GetSystemService(ConnectivityService) as ConnectivityManager;
			if (network_manager.ActiveNetworkInfo == null) {
				Toast.MakeText(this, "No internet connection", ToastLength.Short).Show();
			} else {
				Toast.MakeText(this, "Server error", ToastLength.Short).Show();
			}
		}




		public void changeContentFragment(string type, string subtype) {
			RunOnUiThread(() => { setIsLoading(true); });
			Bundle fragment_args = new Bundle();
			fragment_args.PutString("type", type);
			fragment_args.PutString("subtype", subtype);


			Fragment content_fragment;
			if (type == "movies" || type == "tv_shows" || type == "search" || type == "discover" && subtype != "") {
				content_fragment = new ShowListFragment();
			} else if (type == "discover" && subtype == "") {
				content_fragment = new DiscoverFragment();
			} else if (type == "watchlist"){
				content_fragment = new WatchlistFragment();
			} else {
				content_fragment = new RatedShowsFragment();
			}
			content_fragment.Arguments = fragment_args;


			SupportFragmentManager.BeginTransaction().Replace(Resource.Id.main_activity_fragment_frame, content_fragment)
				.SetTransition(FragmentTransaction.TransitFragmentFade).AddToBackStack(null).Commit();
		}




		public void getRecommendation() {
			RunOnUiThread(() => setIsLoading(true));


			HashSet<string> known_show_ids = UserData.getKnownShowIds();
			Random randomizer = new Random();
			List<Show> shows = null;
			Show recommended_show = null;
			string recommended_show_type = null;


			int page_count = 9999;
			List<RatedShowSerialized> five_star_shows = UserData.getFiveStarShows();
			while (five_star_shows.Any()) {
				int i_show = randomizer.Next(0, five_star_shows.Count - 1);


				string category = five_star_shows[i_show].Id + "/similar";
				for (int i_page = 1; i_page < page_count; ++i_page) {

					Tuple<List<Show>, int> show_data = null;
					if (five_star_shows[i_show].Type == ShowType.Movie.ToString()) {
						show_data = DataProvider.getMovies(category, i_page).Result;
						recommended_show_type = "movie";
					} else {
						show_data = DataProvider.getTvShows(category, i_page).Result;
						recommended_show_type = "tv_show";
					}
					

					if (show_data == null) {
						RunOnUiThread(() => {
							setIsLoading(false);
							showNetworkError();
						});
						return;
					}


					shows = show_data.Item1;
					page_count = show_data.Item2 / DataProvider.SHOWS_PER_PAGE;
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
				List<RatedShowSerialized> four_star_shows = UserData.getFourStarShows();
				while (four_star_shows.Any()) {
					int i_show = randomizer.Next(0, four_star_shows.Count - 1);


					string category = four_star_shows[i_show].Id + "/similar";
					for (int i_page = 1; i_page < page_count; ++i_page) {

						Tuple<List<Show>, int> show_data = null;
						if (four_star_shows[i_show].Type == ShowType.Movie.ToString()) {
							show_data = DataProvider.getMovies(category, i_page).Result;
							recommended_show_type = "movie";
						} else {
							show_data = DataProvider.getTvShows(category, i_page).Result;
							recommended_show_type = "tv_show";
						}


						if (show_data == null) {
							RunOnUiThread(() => {
								setIsLoading(false);
								showNetworkError();
							});
							return;
						}


						shows = show_data.Item1;
						page_count = show_data.Item2 / DataProvider.SHOWS_PER_PAGE;
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
						show_data = DataProvider.getMovies("popular", i_page).Result;
						recommended_show_type = "movie";
					} else {
						show_data = DataProvider.getTvShows("popular", i_page).Result;
						recommended_show_type = "tv_show";
					}


					if (show_data == null) {
						RunOnUiThread(() => {
							setIsLoading(false);
							showNetworkError();
						});
						return;
					}


					shows = show_data.Item1;
					page_count = show_data.Item2 / DataProvider.SHOWS_PER_PAGE;
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
				RunOnUiThread(() => {
					setIsLoading(false);
					showNetworkError();
				});
				return;
			}


			UserData.addToAlreadyRecommendedShows(recommended_show);
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


			SupportFragmentManager.BeginTransaction().Replace(Resource.Id.main_activity_fragment_frame, details_fragment)
				.SetTransition(FragmentTransaction.TransitFragmentFade).AddToBackStack(null).Commit();
		}




		public void submitSearch(string query) {
			changeContentFragment("search", query);
		}




		public void setToolbarTitle(string title) {
			_toolbar.Title = title;
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
				_toolbar.Title = title_a;
			} else {
				_toolbar.Title = title_a + ": " + title_b;
			}
		}




		protected override void OnCreate(Bundle saved_instance_state) {
			base.OnCreate(saved_instance_state);
			UserData = new UserData();


			DataProvider = new DataProvider();
			SetContentView(Resource.Layout.main_activity);


			_loading_view = FindViewById<ImageView>(Resource.Id.show_list_loading);
			((AnimationDrawable) _loading_view.Background).Start();
			setIsLoading(true);


			_toolbar = FindViewById<Toolbar>(Resource.Id.main_activity_toolbar);
			if (saved_instance_state != null) {
				_toolbar.Title = saved_instance_state.GetString("toolbar_title");
			}
			SetSupportActionBar(_toolbar);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);


			_drawer_layout = FindViewById<DrawerLayout>(Resource.Id.main_activity_drawer_layout);
			_drawer_toggle = new ActionBarDrawerToggle(this, _drawer_layout, _toolbar, Android.Resource.Drawable.IcMenuDirections,
				Android.Resource.Drawable.IcMenuDirections);
			_drawer_layout.AddDrawerListener(_drawer_toggle);


			LinearLayout drawer_menu = FindViewById<LinearLayout>(Resource.Id.main_activity_navigation_menu);
			LayoutTransition drawer_menu_transition = new LayoutTransition();
			drawer_menu_transition.DisableTransitionType(LayoutTransitionType.Disappearing);
			drawer_menu.LayoutTransition = drawer_menu_transition;


			MenuOnClickListener menu_on_click_listener = new MenuOnClickListener(this, _drawer_layout);
			for (int i_menu_children = 0; i_menu_children < drawer_menu.ChildCount; ++i_menu_children) {
				drawer_menu.GetChildAt(i_menu_children).SetOnClickListener(menu_on_click_listener);
			}


			if (SupportFragmentManager.FindFragmentById(Resource.Id.main_activity_fragment_frame) == null) {
				ShowListFragment content_frag = new ShowListFragment();
				Bundle fragment_args = new Bundle();
				fragment_args.PutString("type", "movies");
				fragment_args.PutString("subtype", "popular");
				content_frag.Arguments = fragment_args;
				SupportFragmentManager.BeginTransaction().Add(Resource.Id.main_activity_fragment_frame, content_frag, null).Commit();
			}
		}




		public override bool OnCreateOptionsMenu(IMenu menu) {
			MenuInflater.Inflate(Resource.Menu.main_activity_toolbar_menu, menu);
			buildSearchView(menu);
			return true;
		}




		private void buildSearchView(IMenu menu) {
			IMenuItem search_item = menu.FindItem(Resource.Id.toolbar_menu_search);
			SearchView search_view = (SearchView) search_item.ActionView;
			search_view.QueryHint = "Movie or Tv show";
			search_view.SetOnQueryTextListener(new SearchQueryTextListener(this, search_item));
		}




		protected override void OnPostCreate(Bundle saved_instance_state) {
			_drawer_toggle.SyncState();
			base.OnPostCreate(saved_instance_state);
		}




		protected override void OnSaveInstanceState(Bundle saved_instance_state) {
			saved_instance_state.PutString("toolbar_title", _toolbar.Title);
			base.OnSaveInstanceState(saved_instance_state);
		}




		public override void OnBackPressed() {
			if (_drawer_layout.IsDrawerOpen(GravityCompat.Start)) {
				_drawer_layout.CloseDrawer(GravityCompat.Start);
				return;
			}


			var fragments = SupportFragmentManager.Fragments;
			for (int i_fragment = 0; i_fragment < fragments.Count; ++i_fragment) {
				if (SupportFragmentManager.Fragments[i_fragment] is IBackButtonHandler back_button_handler) {
					if (back_button_handler.handleBackButtonPress()) {
						return;
					}
				}
			}


			if (SupportFragmentManager.BackStackEntryCount > 0) {
				clearLoading();
				SupportFragmentManager.PopBackStack();
				return;
			}


			base.OnBackPressed();
		}




		private void clearLoading() {
			_loading_count = 0;
			Task.Delay(200).ContinueWith(a => RunOnUiThread(() => _loading_view.Visibility = ViewStates.Gone));
		}




		public override bool DispatchTouchEvent(MotionEvent motion_event) {
			var fragments = SupportFragmentManager.Fragments;
			for (int i_fragment = 0; i_fragment < fragments.Count; ++i_fragment) {
				if (SupportFragmentManager.Fragments[i_fragment] is ITouchHandler back_button_handler) {
					back_button_handler.handleTouch(motion_event);
				}
			}


			return base.DispatchTouchEvent(motion_event);
		}
	}
}