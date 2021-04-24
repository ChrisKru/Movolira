using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Object = Java.Lang.Object;




namespace Movolira.Pages.SettingsPage {
	public class ThemeSpinnerAdapter : BaseAdapter, ISpinnerAdapter {
		private readonly string[] THEME_NAMES = {
			"EMERALD",
			"BLUE",
			"VIOLET",
			"AMARANTH",
			"FIREBRICK",
			"TANGERINE",
		};
		private readonly string[] THEME_COLORS = {
			"#009987",
			"#176DC3",
			"#501460",
			"#AB154C",
			"#B82727",
			"#E88000",
		};


		public override int Count => 6;
		private MainActivity _main_activity;




		public ThemeSpinnerAdapter(MainActivity main_activity) {
			this._main_activity = main_activity;
		}




		public override Object GetItem(int position) {
			return this.THEME_NAMES[position];
		}




		public override long GetItemId(int position) {
			return position;
		}




		public override View GetView(int position, View convert_view, ViewGroup parent_view) {
			TextView dropdown_item_view = convert_view as TextView;
			if (dropdown_item_view == null) {
				dropdown_item_view = (TextView)this._main_activity.LayoutInflater
					.Inflate(Resource.Drawable.theme_spinner_item, null);
			}


			dropdown_item_view.Text = this.THEME_NAMES[position];
			return dropdown_item_view;
		}




		public override View GetDropDownView(int position, View convert_view, ViewGroup parent_view) {
			// Hacky way, used to hide the currently selected theme from the dropdown list.
			// Doesn't reuse views, could be optimalized by saving the last theme index in local database
			// (in event of performance issues).
			CheckedTextView item_view = (CheckedTextView)this._main_activity.LayoutInflater.Inflate(
				Resource.Drawable.theme_spinner_dropdown_item, null);
			if(position == this._main_activity.SettingsProvider.getCurrentThemeIndex()) {
				item_view.SetHeight(0);
			}


			// Scrollbar glitches out on dropdown items that are 'hidden', which is why it's disabled.
			// The default Spinner class doesn't allow access to the pop-up ListView that holds the dropdown items,
			// which is why the scrolldown is disabled here.
			parent_view.VerticalScrollBarEnabled = false;
			item_view.Text = this.THEME_NAMES[position];
			item_view.Background = new ColorDrawable(Color.ParseColor(this.THEME_COLORS[position]));
			return item_view;
		}
	}
}