using System.Globalization;
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
using Newtonsoft.Json;
using AlertDialog = Android.App.AlertDialog;
using Debug = System.Diagnostics.Debug;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using SearchView = Android.Support.V7.Widget.SearchView;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Movolira {
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
	public class MainActivity : AppCompatActivity {
		public DataProvider DataProvider { get; private set; }
		public bool IsLoading { get; private set; }
		private DrawerLayout _drawer_layout;
		private ActionBarDrawerToggle _drawer_toggle;
		private AlertDialog _filter_dialog;
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
			ShowListFragment content_fragment = new ShowListFragment();
			Bundle fragment_args = new Bundle();
			fragment_args.PutString("type", type);
			fragment_args.PutString("subtype", subtype);
			content_fragment.Arguments = fragment_args;
			if (type == "movies" || type == "tv_shows") {
				SupportFragmentManager.PopBackStack(null, (int) PopBackStackFlags.Inclusive);
				SupportFragmentManager.BeginTransaction().Replace(Resource.Id.main_activity_fragment_frame, content_fragment)
					.SetTransition(FragmentTransaction.TransitFragmentFade).Commit();
			} else {
				SupportFragmentManager.BeginTransaction().Replace(Resource.Id.main_activity_fragment_frame, content_fragment)
					.SetTransition(FragmentTransaction.TransitFragmentFade).AddToBackStack(null).Commit();
			}
		}

		public void submitSearch(string query) {
			changeContentFragment("search", query);
		}

		public void setToolbarTitle(string title) {
			_toolbar.Title = title;
		}

		public void setToolbarTitle(string type, string subtype) {
			if (type == "search") {
				type = "Search";
			} else {
				TextInfo text_info = new CultureInfo("en-US", false).TextInfo;
				type = type.Replace("_", " ");
				type = text_info.ToTitleCase(type);
				subtype = subtype.Replace("_", " ");
				subtype = text_info.ToTitleCase(subtype);
			}
			_toolbar.Title = type + ": " + subtype;
		}

		protected override void OnCreate(Bundle saved_app_state) {
			base.OnCreate(saved_app_state);
			if (saved_app_state != null) {
				DataProvider = JsonConvert.DeserializeObject<DataProvider>(saved_app_state.GetString("DataProvider"));
			} else {
				DataProvider = new DataProvider();
			}
			SetContentView(Resource.Layout.main_activity);
			_loading_view = FindViewById<ImageView>(Resource.Id.show_list_loading);
			((AnimationDrawable) _loading_view.Background).Start();
			setIsLoading(true);
			_toolbar = FindViewById<Toolbar>(Resource.Id.main_activity_toolbar);
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
				fragment_args.PutString("subtype", "most_popular");
				content_frag.Arguments = fragment_args;
				SupportFragmentManager.BeginTransaction().Add(Resource.Id.main_activity_fragment_frame, content_frag, null).Commit();
			}
		}

		public override bool OnCreateOptionsMenu(IMenu menu) {
			MenuInflater.Inflate(Resource.Menu.main_activity_toolbar_menu, menu);
			IMenuItem search_item = menu.FindItem(Resource.Id.toolbar_menu_search);
			SearchView search_view = (SearchView) search_item.ActionView;
			search_view.QueryHint = "Movie or Tv show";
			search_view.SetOnQueryTextListener(new SearchQueryTextListener(this, search_item));
			AlertDialog.Builder dialog_builder = new AlertDialog.Builder(this);
			View filter_dialog_layout = LayoutInflater.Inflate(Resource.Layout.filter_dialog, null);
			var runtime_range_slider = filter_dialog_layout.FindViewById<Xamarin.RangeSlider.RangeSliderControl>(Resource.Id.filter_dialog_runtime_range_slider);
			TextView runtime_range_view = filter_dialog_layout.FindViewById<TextView>(Resource.Id.filter_dialog_runtime_range);
			runtime_range_slider.SetSelectedMaxValue(runtime_range_slider.AbsoluteMaxValue);
			runtime_range_slider.NotifyWhileDragging = true;
			runtime_range_slider.LowerValueChanged += (a, b) => {
				var updated_range_slider = filter_dialog_layout.FindViewById<Xamarin.RangeSlider.RangeSliderControl>(Resource.Id.filter_dialog_runtime_range_slider);
				runtime_range_view.Text = updated_range_slider.GetSelectedMinValue() + "-" + updated_range_slider.GetSelectedMaxValue() + "min";
			};
			runtime_range_slider.UpperValueChanged += (a, b) => {
				var updated_range_slider = filter_dialog_layout.FindViewById<Xamarin.RangeSlider.RangeSliderControl>(Resource.Id.filter_dialog_runtime_range_slider);
				runtime_range_view.Text = updated_range_slider.GetSelectedMinValue() + "-" + updated_range_slider.GetSelectedMaxValue() + "min";
			};
			var rating_range_slider = filter_dialog_layout.FindViewById<Xamarin.RangeSlider.RangeSliderControl>(Resource.Id.filter_dialog_rating_range_slider);
			TextView rating_range_view = filter_dialog_layout.FindViewById<TextView>(Resource.Id.filter_dialog_rating_range);
			rating_range_slider.SetSelectedMaxValue(rating_range_slider.AbsoluteMaxValue);
			rating_range_slider.NotifyWhileDragging = true;
			rating_range_slider.LowerValueChanged += (a, b) => {
				var updated_range_slider = filter_dialog_layout.FindViewById<Xamarin.RangeSlider.RangeSliderControl>(Resource.Id.filter_dialog_rating_range_slider);
				rating_range_view.Text = updated_range_slider.GetSelectedMinValue() + "-" + updated_range_slider.GetSelectedMaxValue();
			};
			rating_range_slider.UpperValueChanged += (a, b) => {
				var updated_range_slider = filter_dialog_layout.FindViewById<Xamarin.RangeSlider.RangeSliderControl>(Resource.Id.filter_dialog_rating_range_slider);
				rating_range_view.Text = updated_range_slider.GetSelectedMinValue() + "-" + updated_range_slider.GetSelectedMaxValue();
			};
			dialog_builder.SetView(filter_dialog_layout);
			_filter_dialog = dialog_builder.Create();
			return true;
		}

		public override bool OnOptionsItemSelected(IMenuItem menu_item) {
			if (menu_item.ItemId == Resource.Id.toolbar_menu_filter) {
				_filter_dialog.Show();
				return true;
			}
			return base.OnOptionsItemSelected(menu_item);
		}

		protected override void OnPostCreate(Bundle saved_app_state) {
			_drawer_toggle.SyncState();
			base.OnPostCreate(saved_app_state);
		}

		public override void OnBackPressed() {
			if (_drawer_layout.IsDrawerOpen(GravityCompat.Start)) {
				_drawer_layout.CloseDrawer(GravityCompat.Start);
			} else if (SupportFragmentManager.BackStackEntryCount > 0) {
				clearLoading();
				SupportFragmentManager.PopBackStack();
			} else {
				var fragments = SupportFragmentManager.Fragments;
				for (int i_fragment = 0; i_fragment < fragments.Count; ++i_fragment) {
					if (SupportFragmentManager.Fragments[i_fragment] is IBackButtonHandler back_button_handler) {
						if (back_button_handler.handleBackButtonPress()) {
							return;
						}
					}
				}
				base.OnBackPressed();
			}
		}

		protected override void OnSaveInstanceState(Bundle new_app_state) {
			new_app_state.PutString("DataProvider", JsonConvert.SerializeObject(DataProvider));
			base.OnSaveInstanceState(new_app_state);
		}

		private void clearLoading() {
			_loading_count = 0;
			Task.Delay(200).ContinueWith(a => RunOnUiThread(() => _loading_view.Visibility = ViewStates.Gone));
		}
	}
}