using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using Xamarin.RangeSlider;

namespace Movolira {
	internal class FilterDialog {
		private const int YEARS_MIN_VALUE = 1900;
		private const int YEARS_MAX_VALUE = 2050;
		private const int DEFAULT_YEARS_START_VALUE = 1990;
		private const int DEFAULT_YEARS_END_VALUE = 2020;
		private readonly List<ToggleButton> _genre_buttons;
		private readonly MainActivity _main_activity;


		private AlertDialog _dialog;
		private TextView _rating_range_view;
		private TextView _runtime_range_view;
		private NumberPicker _years_end_picker;
		private NumberPicker _years_start_picker;


		public FilterDialog(MainActivity main_activity) {
			_main_activity = main_activity;
			_genre_buttons = new List<ToggleButton>();
			buildFilterDialog(main_activity);
		}


		public void showDialog() {
			_dialog.Show();
		}


		private void buildFilterDialog(MainActivity main_activity) {
			AlertDialog.Builder dialog_builder = new AlertDialog.Builder(main_activity);
			View filter_dialog_layout = main_activity.LayoutInflater.Inflate(Resource.Layout.filter_dialog, null);


			buildGenreButtons(filter_dialog_layout);
			buildDialogRuntimeRange(filter_dialog_layout);
			buildDialogRatingRange(filter_dialog_layout);
			buildYearsNumberPickers(filter_dialog_layout);
			buildCancelButton(filter_dialog_layout);
			buildResetButton(filter_dialog_layout);
			buildFilterButton(filter_dialog_layout);


			dialog_builder.SetView(filter_dialog_layout);
			_dialog = dialog_builder.Create();
		}


		private void buildGenreButtons(View filter_dialog_layout) {
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_adventure));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_animation));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_anime));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_comedy));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_crime));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_disaster));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_documentary));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_drama));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_eastern));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_family));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_fan_film));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_fantasy));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_film_noir));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_history));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_holiday));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_horror));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_indie));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_music));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_musical));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_mystery));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_road));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_romance));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_science_fiction));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_short));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_sports));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_sporting_event));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_suspense));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_thriller));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_tv_movie));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_war));
			_genre_buttons.Add(filter_dialog_layout.FindViewById<ToggleButton>(Resource.Id.filter_dialog_genre_button_western));


			Button all_genres_button = filter_dialog_layout.FindViewById<Button>(Resource.Id.filter_dialog_all_genres_button);
			all_genres_button.Click += (sender, args) => {
				foreach (ToggleButton genre_button in _genre_buttons) {
					genre_button.Checked = true;
				}
			};


			Button none_genres_button = filter_dialog_layout.FindViewById<Button>(Resource.Id.filter_dialog_none_genres_button);
			none_genres_button.Click += (sender, args) => {
				foreach (ToggleButton genre_button in _genre_buttons) {
					genre_button.Checked = false;
				}
			};
		}


		private void buildDialogRuntimeRange(View filter_dialog_layout) {
			RangeSliderControl runtime_range_slider =
				filter_dialog_layout.FindViewById<RangeSliderControl>(Resource.Id.filter_dialog_runtime_range_slider);
			_runtime_range_view = filter_dialog_layout.FindViewById<TextView>(Resource.Id.filter_dialog_runtime_range);
			runtime_range_slider.SetSelectedMaxValue(runtime_range_slider.AbsoluteMaxValue);
			runtime_range_slider.NotifyWhileDragging = true;


			runtime_range_slider.LowerValueChanged += (a, b) => {
				RangeSliderControl updated_range_slider =
					filter_dialog_layout.FindViewById<RangeSliderControl>(Resource.Id.filter_dialog_runtime_range_slider);
				_runtime_range_view.Text = updated_range_slider.GetSelectedMinValue() + "-" + updated_range_slider.GetSelectedMaxValue() + "min";
			};
			runtime_range_slider.UpperValueChanged += (a, b) => {
				RangeSliderControl updated_range_slider =
					filter_dialog_layout.FindViewById<RangeSliderControl>(Resource.Id.filter_dialog_runtime_range_slider);
				_runtime_range_view.Text = updated_range_slider.GetSelectedMinValue() + "-" + updated_range_slider.GetSelectedMaxValue() + "min";
			};
		}


		private void buildDialogRatingRange(View filter_dialog_layout) {
			RangeSliderControl rating_range_slider =
				filter_dialog_layout.FindViewById<RangeSliderControl>(Resource.Id.filter_dialog_rating_range_slider);
			_rating_range_view = filter_dialog_layout.FindViewById<TextView>(Resource.Id.filter_dialog_rating_range);
			rating_range_slider.SetSelectedMaxValue(rating_range_slider.AbsoluteMaxValue);
			rating_range_slider.NotifyWhileDragging = true;


			rating_range_slider.LowerValueChanged += (a, b) => {
				RangeSliderControl updated_range_slider =
					filter_dialog_layout.FindViewById<RangeSliderControl>(Resource.Id.filter_dialog_rating_range_slider);
				_rating_range_view.Text = updated_range_slider.GetSelectedMinValue() + "-" + updated_range_slider.GetSelectedMaxValue();
			};
			rating_range_slider.UpperValueChanged += (a, b) => {
				RangeSliderControl updated_range_slider =
					filter_dialog_layout.FindViewById<RangeSliderControl>(Resource.Id.filter_dialog_rating_range_slider);
				_rating_range_view.Text = updated_range_slider.GetSelectedMinValue() + "-" + updated_range_slider.GetSelectedMaxValue();
			};
		}


		private void buildYearsNumberPickers(View filter_dialog_layout) {
			_years_start_picker = filter_dialog_layout.FindViewById<NumberPicker>(Resource.Id.filter_dialog_years_start_picker);
			_years_start_picker.MinValue = YEARS_MIN_VALUE;
			_years_start_picker.MaxValue = YEARS_MAX_VALUE;
			_years_start_picker.Value = DEFAULT_YEARS_START_VALUE;


			_years_end_picker = filter_dialog_layout.FindViewById<NumberPicker>(Resource.Id.filter_dialog_years_end_picker);
			_years_end_picker.MinValue = YEARS_MIN_VALUE;
			_years_end_picker.MaxValue = YEARS_MAX_VALUE;
			_years_end_picker.Value = DEFAULT_YEARS_END_VALUE;
		}


		private void buildCancelButton(View filter_dialog_layout) {
			Button cancel_button = filter_dialog_layout.FindViewById<Button>(Resource.Id.filter_dialog_cancel_button);
			cancel_button.Click += (sender, args) => { _dialog.Hide(); };
		}


		private void buildResetButton(View filter_dialog_layout) {
			Button reset_button = filter_dialog_layout.FindViewById<Button>(Resource.Id.filter_dialog_reset_button);
			reset_button.Click += (sender, args) => {
				foreach (ToggleButton genre_button in _genre_buttons) {
					genre_button.Checked = true;
				}


				RangeSliderControl runtime_range_slider =
					filter_dialog_layout.FindViewById<RangeSliderControl>(Resource.Id.filter_dialog_runtime_range_slider);
				runtime_range_slider.SetSelectedMaxValue(runtime_range_slider.AbsoluteMaxValue);
				runtime_range_slider.SetSelectedMinValue(runtime_range_slider.AbsoluteMinValue);
				_runtime_range_view.Text = runtime_range_slider.AbsoluteMinValue + "-" + runtime_range_slider.AbsoluteMaxValue + "min";


				RangeSliderControl rating_range_slider =
					filter_dialog_layout.FindViewById<RangeSliderControl>(Resource.Id.filter_dialog_rating_range_slider);
				rating_range_slider.SetSelectedMaxValue(rating_range_slider.AbsoluteMaxValue);
				rating_range_slider.SetSelectedMinValue(rating_range_slider.AbsoluteMinValue);
				_rating_range_view.Text = rating_range_slider.AbsoluteMinValue + "-" + rating_range_slider.AbsoluteMaxValue;
				_years_start_picker.Value = DEFAULT_YEARS_START_VALUE;
				_years_end_picker.Value = DEFAULT_YEARS_END_VALUE;
			};
		}


		private void buildFilterButton(View filter_dialog_layout) {
			Button filter_button = filter_dialog_layout.FindViewById<Button>(Resource.Id.filter_dialog_filter_button);
			filter_button.Click += (sender, args) => {
				string genres_query = "genres=";
				bool none_genres = true;
				foreach (ToggleButton genre_button in _genre_buttons) {
					if (genre_button.Checked) {
						if (none_genres) {
							none_genres = false;
						}
						genres_query += genre_button.TextOn.Replace(' ', '-').ToLower() + ",";
					}
				}
				if (none_genres) {
					genres_query += "NONE";
				}


				string runtime_query = "runtimes=";
				RangeSliderControl runtime_range_slider =
					filter_dialog_layout.FindViewById<RangeSliderControl>(Resource.Id.filter_dialog_runtime_range_slider);
				runtime_query += runtime_range_slider.GetSelectedMinValue() + "-" + runtime_range_slider.GetSelectedMaxValue();


				string ratings_query = "ratings=";
				ratings_query += _rating_range_view.Text;


				string years_query = "years=";
				if (_years_start_picker.Value < _years_end_picker.Value) {
					years_query += _years_start_picker.Value.ToString() + "-" + _years_end_picker.Value.ToString();
				} else {
					years_query += _years_end_picker.Value.ToString() + "-" + _years_start_picker.Value.ToString();
				}
				string filter_query = genres_query + "&" + runtime_query + "&" + ratings_query + "&" + years_query;


				_main_activity.submitFilter(filter_query);
				_dialog.Hide();
			};
		}
	}
}