using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Newtonsoft.Json;
using Xamarin.RangeSlider;

namespace Movolira {
	public class AdvancedSearchFragment : Fragment, IBackButtonHandler, ITouchHandler {
		private const int YEARS_MIN_VALUE = 1900;
		private const int YEARS_MAX_VALUE = 2050;
		private const int DEFAULT_YEARS_START_VALUE = 1990;
		private const int DEFAULT_YEARS_END_VALUE = 2020;


		private View _layout;
		private List<ToggleButton> _genre_buttons;
		private EditText _keywords_textbox;
		private MainActivity _main_activity;
		private TextView _rating_range_view;
		private TextView _runtime_range_view;
		private NumberPicker _years_end_picker;
		private NumberPicker _years_start_picker;




		public override void OnCreate(Bundle saved_instance_state) {
			base.OnCreate(saved_instance_state);
		}




		public override void OnAttach(Context activity) {
			_main_activity = (MainActivity) activity;
			_genre_buttons = new List<ToggleButton>();
			base.OnAttach(activity);
		}




		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle saved_instance_state) {
			_layout = inflater.Inflate(Resource.Layout.advanced_search, container, false);
			

			Task.Run(() => buildGenreButtons(_layout, inflater, saved_instance_state));
			buildKeywordsTextBox(_layout);
			buildRuntimeRange(_layout, saved_instance_state);
			buildRatingRange(_layout, saved_instance_state);
			buildYearsNumberPickers(_layout, saved_instance_state);
			buildResetButton(_layout);
			buildSearchButton(_layout);


			return _layout;
		}




		private async void buildGenreButtons(View advanced_search_layout, LayoutInflater inflater, Bundle saved_instance_state) {
			var genre_list = await _main_activity.DataProvider.getGenreList();
			_main_activity.RunOnUiThread(() => {
				ViewGroup genre_button_layout = advanced_search_layout.FindViewById<ViewGroup>(Resource.Id.advanced_search_genre_buttons);
				foreach (var genre_pair in genre_list) {
					if (genre_pair.Value.Contains("&")) {
						continue;
					}


					ToggleButton genre_button =
						(ToggleButton) inflater.Inflate(Resource.Layout.advanced_search_genre_button, genre_button_layout, false);
					genre_button_layout.AddView(genre_button);
					genre_button.TextOn = genre_pair.Value;
					genre_button.TextOff = genre_pair.Value;
					genre_button.Checked = true;
					_genre_buttons.Add(genre_button);
				}


				if (saved_instance_state != null) {
					int[] unchecked_buttons_indexes = saved_instance_state.GetIntArray("unchecked_buttons_indexes");
					foreach (int i_unchecked_button in unchecked_buttons_indexes) {
						_genre_buttons[i_unchecked_button].Checked = false;
					}
				}

				Button all_genres_button = advanced_search_layout.FindViewById<Button>(Resource.Id.advanced_search_all_genres_button);
				all_genres_button.Click += (sender, args) => {
					foreach (ToggleButton genre_button in _genre_buttons) {
						genre_button.Checked = true;
					}
				};


				Button none_genres_button = advanced_search_layout.FindViewById<Button>(Resource.Id.advanced_search_none_genres_button);
				none_genres_button.Click += (sender, args) => {
					foreach (ToggleButton genre_button in _genre_buttons) {
						genre_button.Checked = false;
					}
				};


				_main_activity.setIsLoading(false);
			});
		}




		private void buildKeywordsTextBox(View advanced_search_layout) {
			_keywords_textbox = advanced_search_layout.FindViewById<EditText>(Resource.Id.advanced_search_keywords_textbox);
			_keywords_textbox.SetOnEditorActionListener(new TextBoxOnActionListener(_keywords_textbox));
		}




		private void buildRuntimeRange(View advanced_search_layout, Bundle saved_instance_state) {
			RangeSliderControl runtime_range_slider =
				advanced_search_layout.FindViewById<RangeSliderControl>(Resource.Id.advanced_search_runtime_range_slider);
			_runtime_range_view = advanced_search_layout.FindViewById<TextView>(Resource.Id.advanced_search_runtime_range);
			runtime_range_slider.SetSelectedMaxValue(runtime_range_slider.AbsoluteMaxValue);
			runtime_range_slider.NotifyWhileDragging = true;


			if (saved_instance_state == null) {
				_runtime_range_view.Text = runtime_range_slider.GetSelectedMinValue() + "-" + runtime_range_slider.GetSelectedMaxValue() + "min";
			} else {
				float runtime_range_min = saved_instance_state.GetFloat("runtime_range_min");
				float runtime_range_max = saved_instance_state.GetFloat("runtime_range_max");
				_runtime_range_view.Text = runtime_range_min + "-" + runtime_range_max + "min";
			}


			runtime_range_slider.LowerValueChanged += (a, b) => {
				RangeSliderControl updated_range_slider =
					advanced_search_layout.FindViewById<RangeSliderControl>(Resource.Id.advanced_search_runtime_range_slider);
				_runtime_range_view.Text = updated_range_slider.GetSelectedMinValue() + "-" + updated_range_slider.GetSelectedMaxValue() + "min";
			};


			runtime_range_slider.UpperValueChanged += (a, b) => {
				RangeSliderControl updated_range_slider =
					advanced_search_layout.FindViewById<RangeSliderControl>(Resource.Id.advanced_search_runtime_range_slider);
				_runtime_range_view.Text = updated_range_slider.GetSelectedMinValue() + "-" + updated_range_slider.GetSelectedMaxValue() + "min";
			};
		}




		private void buildRatingRange(View advanced_search_layout, Bundle saved_instance_state) {
			RangeSliderControl rating_range_slider =
				advanced_search_layout.FindViewById<RangeSliderControl>(Resource.Id.advanced_search_rating_range_slider);
			_rating_range_view = advanced_search_layout.FindViewById<TextView>(Resource.Id.advanced_search_rating_range);
			rating_range_slider.SetSelectedMaxValue(rating_range_slider.AbsoluteMaxValue);
			rating_range_slider.NotifyWhileDragging = true;


			if (saved_instance_state == null) {
				_rating_range_view.Text = rating_range_slider.GetSelectedMinValue() + "-" + rating_range_slider.GetSelectedMaxValue();
			} else {
				float rating_range_min = saved_instance_state.GetFloat("rating_range_min");
				float rating_range_max = saved_instance_state.GetFloat("rating_range_max");
				_rating_range_view.Text = rating_range_min + "-" + rating_range_max;
			}


			rating_range_slider.LowerValueChanged += (a, b) => {
				RangeSliderControl updated_range_slider =
					advanced_search_layout.FindViewById<RangeSliderControl>(Resource.Id.advanced_search_rating_range_slider);
				_rating_range_view.Text = updated_range_slider.GetSelectedMinValue() + "-" + updated_range_slider.GetSelectedMaxValue();
			};


			rating_range_slider.UpperValueChanged += (a, b) => {
				RangeSliderControl updated_range_slider =
					advanced_search_layout.FindViewById<RangeSliderControl>(Resource.Id.advanced_search_rating_range_slider);
				_rating_range_view.Text = updated_range_slider.GetSelectedMinValue() + "-" + updated_range_slider.GetSelectedMaxValue();
			};
		}




		private void buildYearsNumberPickers(View advanced_search_layout, Bundle saved_instance_state) {
			_years_start_picker = advanced_search_layout.FindViewById<NumberPicker>(Resource.Id.advanced_search_years_start_picker);
			_years_start_picker.MinValue = YEARS_MIN_VALUE;
			_years_start_picker.MaxValue = YEARS_MAX_VALUE;


			if (saved_instance_state == null) {
				_years_start_picker.Value = DEFAULT_YEARS_START_VALUE;
			} else {
				_years_start_picker.Value = saved_instance_state.GetInt("years_start_picker_value");
			}


			_years_end_picker = advanced_search_layout.FindViewById<NumberPicker>(Resource.Id.advanced_search_years_end_picker);
			_years_end_picker.MinValue = YEARS_MIN_VALUE;
			_years_end_picker.MaxValue = YEARS_MAX_VALUE;


			if (saved_instance_state == null) {
				_years_end_picker.Value = DEFAULT_YEARS_END_VALUE;
			} else {
				_years_end_picker.Value = saved_instance_state.GetInt("years_end_picker_value");
			}
		}




		private void buildResetButton(View advanced_search_layout) {
			Button reset_button = advanced_search_layout.FindViewById<Button>(Resource.Id.advanced_search_reset_button);
			reset_button.Click += (sender, args) => {
				foreach (ToggleButton genre_button in _genre_buttons) {
					genre_button.Checked = true;
				}


				RangeSliderControl runtime_range_slider =
					advanced_search_layout.FindViewById<RangeSliderControl>(Resource.Id.advanced_search_runtime_range_slider);
				runtime_range_slider.SetSelectedMaxValue(runtime_range_slider.AbsoluteMaxValue);
				runtime_range_slider.SetSelectedMinValue(runtime_range_slider.AbsoluteMinValue);
				_runtime_range_view.Text = runtime_range_slider.AbsoluteMinValue + "-" + runtime_range_slider.AbsoluteMaxValue + "min";


				RangeSliderControl rating_range_slider =
					advanced_search_layout.FindViewById<RangeSliderControl>(Resource.Id.advanced_search_rating_range_slider);
				rating_range_slider.SetSelectedMaxValue(rating_range_slider.AbsoluteMaxValue);
				rating_range_slider.SetSelectedMinValue(rating_range_slider.AbsoluteMinValue);
				_rating_range_view.Text = rating_range_slider.AbsoluteMinValue + "-" + rating_range_slider.AbsoluteMaxValue;
				_years_start_picker.Value = DEFAULT_YEARS_START_VALUE;
				_years_end_picker.Value = DEFAULT_YEARS_END_VALUE;
			};
		}




		private void buildSearchButton(View advanced_search_layout) {
			Button search_button = advanced_search_layout.FindViewById<Button>(Resource.Id.advanced_search_search_button);
			search_button.Click += (sender, args) => {
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
					advanced_search_layout.FindViewById<RangeSliderControl>(Resource.Id.advanced_search_runtime_range_slider);
				runtime_query += runtime_range_slider.GetSelectedMinValue() + "-" + runtime_range_slider.GetSelectedMaxValue();


				string ratings_query = "ratings=";
				ratings_query += _rating_range_view.Text;


				string years_query = "years=";
				if (_years_start_picker.Value < _years_end_picker.Value) {
					years_query += _years_start_picker.Value.ToString() + "-" + _years_end_picker.Value.ToString();
				} else {
					years_query += _years_end_picker.Value.ToString() + "-" + _years_start_picker.Value.ToString();
				}
				string search_query = genres_query + "&" + runtime_query + "&" + ratings_query + "&" + years_query;
			};
		}




		public override void OnSaveInstanceState(Bundle new_app_state) {
			List<int> unchecked_buttons_indexes = new List<int>();
			for (int i_genre_button = 0; i_genre_button < _genre_buttons.Count; ++i_genre_button) {
				if (_genre_buttons[i_genre_button].Checked == false) {
					unchecked_buttons_indexes.Add(i_genre_button);
				}
			}


			RangeSliderControl runtime_range_slider = _layout.FindViewById<RangeSliderControl>(Resource.Id.advanced_search_runtime_range_slider);
			RangeSliderControl rating_range_slider = _layout.FindViewById<RangeSliderControl>(Resource.Id.advanced_search_rating_range_slider);


			new_app_state.PutIntArray("unchecked_buttons_indexes", unchecked_buttons_indexes.ToArray());
			new_app_state.PutFloat("runtime_range_min", runtime_range_slider.GetSelectedMinValue());
			new_app_state.PutFloat("runtime_range_max", runtime_range_slider.GetSelectedMaxValue());
			new_app_state.PutFloat("rating_range_min", rating_range_slider.GetSelectedMinValue());
			new_app_state.PutFloat("rating_range_max", rating_range_slider.GetSelectedMaxValue());
			new_app_state.PutInt("years_start_picker_value", _years_start_picker.Value);
			new_app_state.PutInt("years_end_picker_value", _years_end_picker.Value);


			base.OnSaveInstanceState(new_app_state);
		}




		public bool handleBackButtonPress() {
			if (_keywords_textbox.IsFocused) {
				_keywords_textbox.ClearFocus();
				((ViewGroup) _keywords_textbox.Parent).RequestFocus();
				return true;
			}


			return false;
		}




		public void handleTouch(MotionEvent motion_event) {
			_keywords_textbox.ClearFocus();
			((ViewGroup)_keywords_textbox.Parent).RequestFocus();
			InputMethodManager input_manager = (InputMethodManager) _main_activity.GetSystemService(Context.InputMethodService);
			input_manager.HideSoftInputFromWindow(_keywords_textbox.WindowToken, 0);
		}
	}
}