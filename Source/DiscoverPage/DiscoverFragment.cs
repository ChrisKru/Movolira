using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Xamarin.RangeSlider;

namespace Movolira {
	public class DiscoverFragment : Fragment {
		private const int YEARS_MIN_VALUE = 1900;
		private const int YEARS_MAX_VALUE = 2050;
		private const int DEFAULT_YEARS_START_VALUE = 1990;
		private const int DEFAULT_YEARS_END_VALUE = 2020;
		private const int RATING_MIN_VALUE = 0;
		private const int RATING_MAX_VALUE = 100;
		private const int RUNTIME_MIN_VALUE = 0;
		private const int RUNTIME_MAX_VALUE = 300;


		private MainActivity _main_activity;
		private Dictionary<string, string> _genre_ids;
		private Bundle _frag_saved_state;


		private View _layout;
		private List<ToggleButton> _genre_buttons;
		private TextView _rating_range_view;
		private TextView _runtime_range_view;
		private NumberPicker _years_end_picker;
		private NumberPicker _years_start_picker;




		public override void OnCreate(Bundle saved_instance_state) {
			base.OnCreate(saved_instance_state);
		}




		public override void OnAttach(Context activity) {
			_main_activity = (MainActivity) activity;
			base.OnAttach(activity);
		}




		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle saved_instance_state) {
			_layout = inflater.Inflate(Resource.Layout.discover_page, container, false);


			if (_frag_saved_state != null) {
				Task.Run(() => buildGenreButtons(_layout, inflater, _frag_saved_state));
				buildRuntimeRange(_layout, _frag_saved_state);
				buildRatingRange(_layout, _frag_saved_state);
				buildResetButton(_layout);
				buildDiscoverButton(_layout);
				buildYearsNumberPickers(_layout, _frag_saved_state);
			} else {
				Task.Run(() => buildGenreButtons(_layout, inflater, saved_instance_state));
				buildRuntimeRange(_layout, saved_instance_state);
				buildRatingRange(_layout, saved_instance_state);
				buildResetButton(_layout);
				buildDiscoverButton(_layout);
				buildYearsNumberPickers(_layout, saved_instance_state);
			}
			
			


			_main_activity.setToolbarTitle("Discover");
			return _layout;
		}




		private async void buildGenreButtons(View discover_page_layout, LayoutInflater inflater, Bundle saved_instance_state) {
			_genre_buttons = new List<ToggleButton>();
			_genre_ids = new Dictionary<string, string>();
			var genre_list = await _main_activity.DataProvider.getGenreList();
			_main_activity.RunOnUiThread(() => {
				ViewGroup genre_button_layout = discover_page_layout.FindViewById<ViewGroup>(Resource.Id.discover_page_genre_buttons);
				if (_genre_ids.Count == 0) {
					foreach (var genre_pair in genre_list) {
						if (genre_pair.Value.Contains("&")) {
							var split_genres = genre_pair.Value.Split(new[] {'&', ' '}, StringSplitOptions.RemoveEmptyEntries);
							foreach (string genre in split_genres) {
								if (_genre_ids.ContainsKey(genre)) {
									_genre_ids[genre] = _genre_ids[genre] + "," + genre_pair.Key;
								} else {
									_genre_ids.Add(genre, genre_pair.Key.ToString());
								}
							}
						}


						if (_genre_ids.ContainsKey(genre_pair.Value)) {
							_genre_ids[genre_pair.Value] = _genre_ids[genre_pair.Value] + "," + genre_pair.Key;
						} else {
							_genre_ids.Add(genre_pair.Value, genre_pair.Key.ToString());
						}
					}
				}


				foreach (var genre_pair in genre_list) {
					if (genre_pair.Value.Contains("&")) {
						continue;
					}


					ToggleButton genre_button =
						(ToggleButton) inflater.Inflate(Resource.Layout.discover_page_genre_button, genre_button_layout, false);
					genre_button_layout.AddView(genre_button);
					genre_button.TextOn = genre_pair.Value;
					genre_button.TextOff = genre_pair.Value;
					genre_button.Checked = true;
					_genre_buttons.Add(genre_button);
				}


				if (saved_instance_state != null) {
					var unchecked_buttons_indexes = saved_instance_state.GetIntArray("unchecked_buttons_indexes");
					foreach (int i_unchecked_button in unchecked_buttons_indexes) {
						_genre_buttons[i_unchecked_button].Checked = false;
					}
				}

				Button all_genres_button = discover_page_layout.FindViewById<Button>(Resource.Id.discover_page_all_genres_button);
				all_genres_button.Click += (sender, args) => {
					foreach (ToggleButton genre_button in _genre_buttons) {
						genre_button.Checked = true;
					}
				};


				Button none_genres_button = discover_page_layout.FindViewById<Button>(Resource.Id.discover_page_none_genres_button);
				none_genres_button.Click += (sender, args) => {
					foreach (ToggleButton genre_button in _genre_buttons) {
						genre_button.Checked = false;
					}
				};


				_main_activity.setIsLoading(false);
			});
		}




		private void buildRuntimeRange(View discover_page_layout, Bundle saved_instance_state) {
			RangeSliderControl runtime_range_slider =
				discover_page_layout.FindViewById<RangeSliderControl>(Resource.Id.discover_page_runtime_range_slider);
			_runtime_range_view = discover_page_layout.FindViewById<TextView>(Resource.Id.discover_page_runtime_range);
			runtime_range_slider.NotifyWhileDragging = true;
			runtime_range_slider.SetSelectedMinValue(RUNTIME_MIN_VALUE);
			runtime_range_slider.SetSelectedMaxValue(RUNTIME_MAX_VALUE);
			

			if (saved_instance_state == null) {
				_runtime_range_view.Text = runtime_range_slider.GetSelectedMinValue() + "-" + runtime_range_slider.GetSelectedMaxValue() + "min";
			} else {
				float runtime_range_min = saved_instance_state.GetFloat("runtime_range_min");
				float runtime_range_max = saved_instance_state.GetFloat("runtime_range_max");
				_runtime_range_view.Text = runtime_range_min + "-" + runtime_range_max + "min";
			}


			runtime_range_slider.LowerValueChanged += (a, b) => {
				RangeSliderControl updated_range_slider =
					discover_page_layout.FindViewById<RangeSliderControl>(Resource.Id.discover_page_runtime_range_slider);
				_runtime_range_view.Text = updated_range_slider.GetSelectedMinValue() + "-" + updated_range_slider.GetSelectedMaxValue() + "min";
			};


			runtime_range_slider.UpperValueChanged += (a, b) => {
				RangeSliderControl updated_range_slider =
					discover_page_layout.FindViewById<RangeSliderControl>(Resource.Id.discover_page_runtime_range_slider);
				_runtime_range_view.Text = updated_range_slider.GetSelectedMinValue() + "-" + updated_range_slider.GetSelectedMaxValue() + "min";
			};


			
		}




		private void buildRatingRange(View discover_page_layout, Bundle saved_instance_state) {
			RangeSliderControl rating_range_slider =
				discover_page_layout.FindViewById<RangeSliderControl>(Resource.Id.discover_page_rating_range_slider);
			_rating_range_view = discover_page_layout.FindViewById<TextView>(Resource.Id.discover_page_rating_range);
			rating_range_slider.SetSelectedMinValue(RATING_MIN_VALUE);
			rating_range_slider.SetSelectedMaxValue(RATING_MAX_VALUE);
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
					discover_page_layout.FindViewById<RangeSliderControl>(Resource.Id.discover_page_rating_range_slider);
				_rating_range_view.Text = updated_range_slider.GetSelectedMinValue() + "-" + updated_range_slider.GetSelectedMaxValue();
			};


			rating_range_slider.UpperValueChanged += (a, b) => {
				RangeSliderControl updated_range_slider =
					discover_page_layout.FindViewById<RangeSliderControl>(Resource.Id.discover_page_rating_range_slider);
				_rating_range_view.Text = updated_range_slider.GetSelectedMinValue() + "-" + updated_range_slider.GetSelectedMaxValue();
			};
		}




		private void buildYearsNumberPickers(View discover_page_layout, Bundle saved_instance_state) {
			_years_start_picker = discover_page_layout.FindViewById<NumberPicker>(Resource.Id.discover_page_years_start_picker);
			_years_start_picker.MinValue = YEARS_MIN_VALUE;
			_years_start_picker.MaxValue = YEARS_MAX_VALUE;


			if (saved_instance_state == null) {
				_years_start_picker.Value = DEFAULT_YEARS_START_VALUE;
			} else {
				_years_start_picker.Value = saved_instance_state.GetInt("years_start_picker_value");
			}


			_years_end_picker = discover_page_layout.FindViewById<NumberPicker>(Resource.Id.discover_page_years_end_picker);
			_years_end_picker.MinValue = YEARS_MIN_VALUE;
			_years_end_picker.MaxValue = YEARS_MAX_VALUE;


			if (saved_instance_state == null) {
				_years_end_picker.Value = DEFAULT_YEARS_END_VALUE;
			} else {
				_years_end_picker.Value = saved_instance_state.GetInt("years_end_picker_value");
			}
		}




		private void buildResetButton(View discover_page_layout) {
			Button reset_button = discover_page_layout.FindViewById<Button>(Resource.Id.discover_page_reset_button);
			reset_button.Click += (sender, args) => {
				resetOptions(discover_page_layout);
			};
		}




		private void resetOptions(View discover_page_layout) {
			foreach (ToggleButton genre_button in _genre_buttons) {
				genre_button.Checked = true;
			}


			RangeSliderControl runtime_range_slider =
				discover_page_layout.FindViewById<RangeSliderControl>(Resource.Id.discover_page_runtime_range_slider);
			runtime_range_slider.SetSelectedMaxValue(RUNTIME_MAX_VALUE);
			runtime_range_slider.SetSelectedMinValue(RUNTIME_MIN_VALUE);
			_runtime_range_view.Text = RUNTIME_MIN_VALUE + "-" + RUNTIME_MAX_VALUE + "min";


			RangeSliderControl rating_range_slider =
				discover_page_layout.FindViewById<RangeSliderControl>(Resource.Id.discover_page_rating_range_slider);
			rating_range_slider.SetSelectedMaxValue(RATING_MAX_VALUE);
			rating_range_slider.SetSelectedMinValue(RATING_MIN_VALUE);
			_rating_range_view.Text = RATING_MIN_VALUE + "-" + RATING_MAX_VALUE;


			_years_start_picker.Value = DEFAULT_YEARS_START_VALUE;
			_years_end_picker.Value = DEFAULT_YEARS_END_VALUE;
		}




		private void buildDiscoverButton(View discover_page_layout) {
			Button discover_button = discover_page_layout.FindViewById<Button>(Resource.Id.discover_page_discover_button);
			discover_button.Click += (sender, args) => {
				string excluded_genres_query = "without_genres=";
				bool no_genres_included = true;
				foreach (ToggleButton genre_button in _genre_buttons) {
					if (!genre_button.Checked) {
						excluded_genres_query += _genre_ids[genre_button.TextOn] + ",";
					} else {
						no_genres_included = false;
					}
				}


				RangeSliderControl runtime_range_slider =
					discover_page_layout.FindViewById<RangeSliderControl>(Resource.Id.discover_page_runtime_range_slider);
				RangeSliderControl rating_range_slider =
					discover_page_layout.FindViewById<RangeSliderControl>(Resource.Id.discover_page_rating_range_slider);


				string runtime_min_query = "with_runtime.gte=" + runtime_range_slider.GetSelectedMinValue();
				string runtime_max_query = "with_runtime.lte=" + runtime_range_slider.GetSelectedMaxValue();
				string rating_min_query = "vote_average.gte=" + rating_range_slider.GetSelectedMinValue() / 10;
				string rating_max_query = "vote_average.lte=" + rating_range_slider.GetSelectedMaxValue() / 10;


				DateTime release_date_min = new DateTime(_years_start_picker.Value, 1, 1);
				DateTime release_date_max = new DateTime(_years_end_picker.Value, 1, 1);
				string release_date_min_query =
					"primary_release_date.gte=" + JsonConvert.SerializeObject(release_date_min).Replace("\"", string.Empty);
				string release_date_max_query =
					"primary_release_date.lte=" + JsonConvert.SerializeObject(release_date_max).Replace("\"", string.Empty);
				string air_date_min_query = "first_air_date.gte=" + JsonConvert.SerializeObject(release_date_min).Replace("\"", string.Empty);
				string air_date_max_query = "first_air_date.lte=" + JsonConvert.SerializeObject(release_date_max).Replace("\"", string.Empty);


				string vote_count_query = "vote_count.gte=20";
				string search_query = runtime_min_query + "&" + runtime_max_query + "&" + rating_min_query + "&" + rating_max_query + "&" +
				                      excluded_genres_query + "&" + release_date_min_query + "&" + release_date_max_query + "&" + air_date_min_query +
				                      "&" + air_date_max_query + "&" + vote_count_query;
				if (no_genres_included) {
					search_query += "&with_genres=98534773957395873497";
				}


				_frag_saved_state = getFragmentStateBundle(null);
				Task.Run(() => _main_activity.changeContentFragment("discover", search_query));
			};
		}




		public override void OnSaveInstanceState(Bundle new_frag_state) {
			getFragmentStateBundle(new_frag_state);
			base.OnSaveInstanceState(new_frag_state);
		}




		private Bundle getFragmentStateBundle(Bundle new_frag_state) {
			if (new_frag_state == null) {
				new_frag_state = new Bundle();
			}


			var unchecked_buttons_indexes = new List<int>();
			for (int i_genre_button = 0; i_genre_button < _genre_buttons.Count; ++i_genre_button) {
				if (_genre_buttons[i_genre_button].Checked == false) {
					unchecked_buttons_indexes.Add(i_genre_button);
				}
			}


			RangeSliderControl runtime_range_slider = _layout.FindViewById<RangeSliderControl>(Resource.Id.discover_page_runtime_range_slider);
			RangeSliderControl rating_range_slider = _layout.FindViewById<RangeSliderControl>(Resource.Id.discover_page_rating_range_slider);


			new_frag_state.PutIntArray("unchecked_buttons_indexes", unchecked_buttons_indexes.ToArray());
			new_frag_state.PutFloat("runtime_range_min", runtime_range_slider.GetSelectedMinValue());
			new_frag_state.PutFloat("runtime_range_max", runtime_range_slider.GetSelectedMaxValue());
			new_frag_state.PutFloat("rating_range_min", rating_range_slider.GetSelectedMinValue());
			new_frag_state.PutFloat("rating_range_max", rating_range_slider.GetSelectedMaxValue());
			new_frag_state.PutInt("years_start_picker_value", _years_start_picker.Value);
			new_frag_state.PutInt("years_end_picker_value", _years_end_picker.Value);


			return new_frag_state;
		}
	}
}