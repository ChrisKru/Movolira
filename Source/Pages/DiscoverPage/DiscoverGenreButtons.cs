using System.Collections.Generic;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;




namespace Movolira.Pages.DiscoverPage {
	public class DiscoverGenreButtons {
		private MainActivity _main_activity;
		private List<ToggleButton> _genre_chip_buttons;




		public DiscoverGenreButtons(MainActivity main_activity, View discover_page_layout,
			LayoutInflater inflater, Bundle saved_instance_state) {


			this._main_activity = main_activity;
			Task.Run(() => this.buildGenreButtons(discover_page_layout, inflater, saved_instance_state));
		}




		private async void buildGenreButtons(View discover_page_layout, LayoutInflater inflater, Bundle saved_instance_state) {
			this._genre_chip_buttons = new List<ToggleButton>();
			bool is_genre_dict_filled = await this._main_activity.GenresProvider.tryFillGenreDict();


			if (!is_genre_dict_filled) {
				this._main_activity.RunOnUiThread(() => {
					this._main_activity.setIsLoading(false);
					this._main_activity.showNetworkError();
				});
				return;
			}


			var genre_id_dict = this._main_activity.GenresProvider.getGenreIdDict();
			this._main_activity.RunOnUiThread(() => {
				this.buildGenreChipButtons(discover_page_layout, inflater, saved_instance_state, genre_id_dict);
				this.buildAllGenresButton(discover_page_layout);
				this.buildNoneGenresButton(discover_page_layout);
				this._main_activity.setIsLoading(false);
			});
		}




		private void buildGenreChipButtons(View discover_page_layout, LayoutInflater inflater,
			Bundle saved_instance_state, SortedDictionary<string, List<int>> genre_id_dict) {


			ViewGroup genre_chip_button_layout = discover_page_layout.FindViewById<ViewGroup>(
					Resource.Id.discover_page_genre_chip_buttons);


			foreach (var genre_name in genre_id_dict.Keys) {
				ToggleButton genre_chip_button = (ToggleButton)inflater.Inflate(
					Resource.Layout.discover_page_genre_chip_button, genre_chip_button_layout, false);
				genre_chip_button_layout.AddView(genre_chip_button);
				genre_chip_button.TextOn = genre_name;
				genre_chip_button.TextOff = genre_name;
				genre_chip_button.Checked = true;
				this._genre_chip_buttons.Add(genre_chip_button);
			}


			if (saved_instance_state != null) {
				var unchecked_button_indexes = saved_instance_state.GetIntArray("unchecked_genre_chip_button_indexes");
				foreach (int i_unchecked_button in unchecked_button_indexes) {
					this._genre_chip_buttons[i_unchecked_button].Checked = false;
				}
			}
		}




		private void buildAllGenresButton(View discover_page_layout) {
			Button all_genres_button = discover_page_layout.FindViewById<Button>(Resource.Id.discover_page_all_genres_button);
			all_genres_button.Click += (sender, args) => {
				foreach (ToggleButton genre_chip_button in this._genre_chip_buttons) {
					genre_chip_button.Checked = true;
				}
			};
		}




		private void buildNoneGenresButton(View discover_page_layout) {
			Button none_genres_button = discover_page_layout.FindViewById<Button>(Resource.Id.discover_page_none_genres_button);
			none_genres_button.Click += (sender, args) => {
				foreach (ToggleButton genre_chip_button in this._genre_chip_buttons) {
					genre_chip_button.Checked = false;
				}
			};
		}




		public void resetOptions() {
			foreach (ToggleButton genre_chip_button in this._genre_chip_buttons) {
				genre_chip_button.Checked = true;
			}
		}




		public string getExcludedGenresQuery() {
			string excluded_genres_query = "without_genres=";
			foreach (ToggleButton genre_chip_button in this._genre_chip_buttons) {
				if (!genre_chip_button.Checked) {
					var excluded_genre_ids = this._main_activity.GenresProvider.getGenreIdsForName(genre_chip_button.TextOn);
					foreach (int genre_id in excluded_genre_ids) {
						excluded_genres_query += genre_id.ToString() + ",";
					}
				}
			}


			return excluded_genres_query;
		}




		public void addViewStateToBundle(Bundle state_bundle) {
			var unchecked_genre_chip_button_indexes = new List<int>();
			for (int i_genre_button = 0; i_genre_button < this._genre_chip_buttons.Count; ++i_genre_button) {
				if (this._genre_chip_buttons[i_genre_button].Checked == false) {
					unchecked_genre_chip_button_indexes.Add(i_genre_button);
				}
			}


			state_bundle.PutIntArray("unchecked_genre_chip_button_indexes", unchecked_genre_chip_button_indexes.ToArray());
		}
	}
}