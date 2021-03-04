using Android.OS;
using Android.Views;
using Android.Widget;
using Xamarin.RangeSlider;




namespace Movolira.Pages.DiscoverPage {
	public class DiscoverRuntimeRangeSlider {
		private const int RUNTIME_RANGE_MIN_VALUE = 0;
		private const int RUNTIME_RANGE_MAX_VALUE = 300;


		private RangeSliderControl _runtime_range_slider;
		private TextView _runtime_range_text_view;




		public DiscoverRuntimeRangeSlider(View discover_page_layout, Bundle saved_instance_state) {
			this.buildRuntimeRange(discover_page_layout, saved_instance_state);
		}




		private void buildRuntimeRange(View discover_page_layout, Bundle saved_instance_state) {
			this._runtime_range_slider =
				discover_page_layout.FindViewById<RangeSliderControl>(Resource.Id.discover_page_runtime_range_slider);
			this._runtime_range_text_view = discover_page_layout.FindViewById<TextView>(Resource.Id.discover_page_runtime_range);
			this._runtime_range_slider.SetSelectedMinValue(RUNTIME_RANGE_MIN_VALUE);
			this._runtime_range_slider.SetSelectedMaxValue(RUNTIME_RANGE_MAX_VALUE);
			this._runtime_range_slider.NotifyWhileDragging = true;


			if (saved_instance_state == null) {
				this._runtime_range_text_view.Text = this._runtime_range_slider.GetSelectedMinValue() + "-"
					+ this._runtime_range_slider.GetSelectedMaxValue() + "min";
			} else {
				float selected_min_value = saved_instance_state.GetFloat("runtime_range_selected_min_value");
				float selected_max_value = saved_instance_state.GetFloat("runtime_range_selected_max_value");
				this._runtime_range_text_view.Text = selected_min_value + "-" + selected_max_value + "min";
			}


			this._runtime_range_slider.LowerValueChanged += (a, b) => {
				this.onRuntimeRangeSliderValueChange();
			};
			this._runtime_range_slider.UpperValueChanged += (a, b) => {
				this.onRuntimeRangeSliderValueChange();
			};
		}




		private void onRuntimeRangeSliderValueChange() {
			this._runtime_range_text_view.Text = this._runtime_range_slider.GetSelectedMinValue() + "-"
				+ this._runtime_range_slider.GetSelectedMaxValue() + "min";
		}




		public void resetOptions() {
			this._runtime_range_slider.SetSelectedMaxValue(RUNTIME_RANGE_MAX_VALUE);
			this._runtime_range_slider.SetSelectedMinValue(RUNTIME_RANGE_MIN_VALUE);
			this._runtime_range_text_view.Text = RUNTIME_RANGE_MIN_VALUE + "-" + RUNTIME_RANGE_MAX_VALUE + "min";
		}




		public string getRuntimeMinMaxQuery() {
			string runtime_min_query = "with_runtime.gte=" + this._runtime_range_slider.GetSelectedMinValue();
			string runtime_max_query = "with_runtime.lte=" + this._runtime_range_slider.GetSelectedMaxValue();
			return runtime_min_query + "&" + runtime_max_query;
		}




		public void addViewStateToBundle(Bundle state_bundle) {
			state_bundle.PutFloat("runtime_range_selected_min_value", this._runtime_range_slider.GetSelectedMinValue());
			state_bundle.PutFloat("runtime_range_selected_max_value", this._runtime_range_slider.GetSelectedMaxValue());
		}
	}
}