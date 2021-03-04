using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;




namespace Movolira.Pages.DiscoverPage {
	public class DiscoverYearsRangePicker {
		private const int YEARS_PICKER_MIN_VALUE = 1900;
		private const int YEARS_PICKER_MAX_VALUE = 2050;
		private const int YEARS_PICKER_START_VALUE = 1990;
		private const int YEARS_PICKER_END_VALUE = 2020;


		private NumberPicker _years_start_range_picker;
		private NumberPicker _years_end_range_picker;




		public DiscoverYearsRangePicker(View discover_page_layout, Bundle saved_instance_state) {
			this.buildYearsRangePickers(discover_page_layout, saved_instance_state);
		}




		private void buildYearsRangePickers(View discover_page_layout, Bundle saved_instance_state) {
			this._years_start_range_picker = discover_page_layout.FindViewById<NumberPicker>(
				Resource.Id.discover_page_years_start_range_picker);
			this._years_start_range_picker.MinValue = YEARS_PICKER_MIN_VALUE;
			this._years_start_range_picker.MaxValue = YEARS_PICKER_MAX_VALUE;


			if (saved_instance_state == null) {
				this._years_start_range_picker.Value = YEARS_PICKER_START_VALUE;
			} else {
				this._years_start_range_picker.Value = saved_instance_state.GetInt("years_picker_start_value");
			}


			this._years_end_range_picker = discover_page_layout.FindViewById<NumberPicker>(
				Resource.Id.discover_page_years_end_range_picker);
			this._years_end_range_picker.MinValue = YEARS_PICKER_MIN_VALUE;
			this._years_end_range_picker.MaxValue = YEARS_PICKER_MAX_VALUE;


			if (saved_instance_state == null) {
				this._years_end_range_picker.Value = YEARS_PICKER_END_VALUE;
			} else {
				this._years_end_range_picker.Value = saved_instance_state.GetInt("years_picker_end_value");
			}
		}




		public void resetOptions() {
			this._years_start_range_picker.Value = YEARS_PICKER_START_VALUE;
			this._years_end_range_picker.Value = YEARS_PICKER_END_VALUE;
		}




		public string getReleaseDateMinMaxQuery() {
			DateTime release_date_min = new DateTime(this._years_start_range_picker.Value, 1, 1);
			DateTime release_date_max = new DateTime(this._years_end_range_picker.Value, 1, 1);


			// 'release_date' is used for movies, 'air_date' is used for tv_shows, by the TMDB API.
			string release_date_min_query =
				"primary_release_date.gte=" + JsonConvert.SerializeObject(release_date_min).Replace("\"", string.Empty);
			string release_date_max_query =
				"primary_release_date.lte=" + JsonConvert.SerializeObject(release_date_max).Replace("\"", string.Empty);
			string air_date_min_query = "first_air_date.gte=" + JsonConvert.SerializeObject(release_date_min)
				.Replace("\"", string.Empty);
			string air_date_max_query = "first_air_date.lte=" + JsonConvert.SerializeObject(release_date_max)
				.Replace("\"", string.Empty);


			return release_date_min_query + "&" + release_date_max_query + "&" + air_date_min_query + "&" + air_date_max_query;
		}




		public void addViewStateToBundle(Bundle state_bundle) {
			state_bundle.PutInt("years_picker_start_value", this._years_start_range_picker.Value);
			state_bundle.PutInt("years_picker_end_value", this._years_end_range_picker.Value);
		}
	}
}