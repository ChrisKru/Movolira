using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;




namespace Movolira.Pages.DiscoverPage {
	public class DiscoverFragment : Fragment {
		private MainActivity _main_activity;
		// Holds instance state independently of the activity lifecycle.
		// The variable is used to recover this view's chosen options, 
		// when the user comes back to this view, from discover query result.
		private Bundle _frag_saved_state;


		private DiscoverGenreButtons _genre_buttons;
		private DiscoverRuntimeRangeSlider _runtime_range_slider;
		private DiscoverRatingRangeSlider _rating_range_slider;
		private DiscoverYearsRangePicker _years_range_picker;




		public override void OnCreate(Bundle saved_instance_state) {
			base.OnCreate(saved_instance_state);
		}




		public override void OnAttach(Context activity) {
			this._main_activity = (MainActivity)activity;
			base.OnAttach(activity);
		}




		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle saved_instance_state) {
			View layout = inflater.Inflate(Resource.Layout.discover_page, container, false);


			// The local '_frag_saved_state' variable is initialized when user clicks the "Discover" button.
			// The variable is used to recover this view's chosen options, 
			// when the user comes back to this view, from discover query result.
			Bundle saved_state;
			if (this._frag_saved_state != null) {
				saved_state = this._frag_saved_state;
			} else {
				saved_state = saved_instance_state;
			}


			this._genre_buttons = new DiscoverGenreButtons(this._main_activity, layout, inflater, saved_state);
			this._runtime_range_slider = new DiscoverRuntimeRangeSlider(layout, saved_state);
			this._rating_range_slider = new DiscoverRatingRangeSlider(layout, saved_state);
			this._years_range_picker = new DiscoverYearsRangePicker(layout, saved_state);


			this.buildResetButton(layout);
			this.buildDiscoverButton(layout);
			this._main_activity.setToolbarTitle("Discover");
			return layout;
		}




		private void buildResetButton(View discover_page_layout) {
			Button reset_button = discover_page_layout.FindViewById<Button>(Resource.Id.discover_page_reset_button);
			reset_button.Click += (sender, args) => {
				this.resetOptions();
			};
		}




		private void resetOptions() {
			this._genre_buttons.resetOptions();
			this._runtime_range_slider.resetOptions();
			this._rating_range_slider.resetOptions();
			this._years_range_picker.resetOptions();
		}




		private void buildDiscoverButton(View discover_page_layout) {
			Button discover_button = discover_page_layout.FindViewById<Button>(Resource.Id.discover_page_discover_button);
			discover_button.Click += (sender, args) => {
				this.discoverShows();
			};
		}




		private void discoverShows() {
			string search_query = this._genre_buttons.getExcludedGenresQuery();
			search_query += "&" + this._runtime_range_slider.getRuntimeMinMaxQuery();
			search_query += "&" + this._rating_range_slider.getRatingMinMaxQuery();
			search_query += "&" + this._years_range_picker.getReleaseDateMinMaxQuery();
			// A minimal vote count threshold is used to filter very obscure or fresh entries from TMDB.
			// A lot of those entries have an unproportionally high rating (because of the small amount of votes),
			// and would be prioritized by rating sort functions.
			// The value '20' seems to be a stable option for this filter.
			search_query += "&vote_count.gte=20";


			this._frag_saved_state = this.getFragmentStateBundle(null);
			Task.Run(() => this._main_activity.changeContentFragment("discover", search_query));
		}




		public override void OnSaveInstanceState(Bundle new_frag_state) {
			this.getFragmentStateBundle(new_frag_state);
			base.OnSaveInstanceState(new_frag_state);
		}




		private Bundle getFragmentStateBundle(Bundle new_frag_state) {
			if (new_frag_state == null) {
				new_frag_state = new Bundle();
			}


			this._genre_buttons.addViewStateToBundle(new_frag_state);
			this._runtime_range_slider.addViewStateToBundle(new_frag_state);
			this._rating_range_slider.addViewStateToBundle(new_frag_state);
			this._years_range_picker.addViewStateToBundle(new_frag_state);
			return new_frag_state;
		}
	}
}