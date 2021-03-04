using Android.OS;
using Android.Views;
using Android.Widget;
using Xamarin.RangeSlider;




namespace Movolira.Pages.DiscoverPage {
	public class DiscoverRatingRangeSlider {
		private const int RATING_RANGE_MIN_VALUE = 0;
		private const int RATING_RANGE_MAX_VALUE = 100;


		private RangeSliderControl _rating_range_slider;
		private TextView _rating_range_text_view;




		public DiscoverRatingRangeSlider(View discover_page_layout, Bundle saved_instance_state) {
			this.buildRatingRange(discover_page_layout, saved_instance_state);
		}




		private void buildRatingRange(View discover_page_layout, Bundle saved_instance_state) {
			this._rating_range_slider = discover_page_layout.FindViewById<RangeSliderControl>(
				Resource.Id.discover_page_rating_range_slider);
			this._rating_range_text_view = discover_page_layout.FindViewById<TextView>(Resource.Id.discover_page_rating_range);
			this._rating_range_slider.SetSelectedMinValue(RATING_RANGE_MIN_VALUE);
			this._rating_range_slider.SetSelectedMaxValue(RATING_RANGE_MAX_VALUE);
			this._rating_range_slider.NotifyWhileDragging = true;


			if (saved_instance_state == null) {
				this._rating_range_text_view.Text = this._rating_range_slider.GetSelectedMinValue() + "-"
					+ this._rating_range_slider.GetSelectedMaxValue();
			} else {
				float selected_min_value = saved_instance_state.GetFloat("rating_range_selected_min_value");
				float selected_max_value = saved_instance_state.GetFloat("rating_range_selected_max_value");
				this._rating_range_text_view.Text = selected_min_value + "-" + selected_max_value;
			}


			this._rating_range_slider.LowerValueChanged += (a, b) => {
				this.onRatingRangeSliderValueChange(discover_page_layout);
			};
			this._rating_range_slider.UpperValueChanged += (a, b) => {
				this.onRatingRangeSliderValueChange(discover_page_layout);
			};
		}




		private void onRatingRangeSliderValueChange(View discover_page_layout) {
			this._rating_range_text_view.Text = this._rating_range_slider.GetSelectedMinValue() + "-"
				+ this._rating_range_slider.GetSelectedMaxValue();
		}




		public void resetOptions() {
			this._rating_range_slider.SetSelectedMaxValue(RATING_RANGE_MAX_VALUE);
			this._rating_range_slider.SetSelectedMinValue(RATING_RANGE_MIN_VALUE);
			this._rating_range_text_view.Text = RATING_RANGE_MIN_VALUE + "-" + RATING_RANGE_MAX_VALUE;
		}




		public string getRatingMinMaxQuery() {
			string rating_min_query = "vote_average.gte=" + this._rating_range_slider.GetSelectedMinValue() / 10;
			string rating_max_query = "vote_average.lte=" + this._rating_range_slider.GetSelectedMaxValue() / 10;
			return rating_min_query + "&" + rating_max_query;
		}




		public void addViewStateToBundle(Bundle state_bundle) {
			state_bundle.PutFloat("rating_range_selected_min_value", this._rating_range_slider.GetSelectedMinValue());
			state_bundle.PutFloat("rating_range_selected_max_value", this._rating_range_slider.GetSelectedMaxValue());
		}
	}
}