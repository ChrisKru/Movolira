using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Object = Java.Lang.Object;




namespace Movolira.Pages.SettingsPage {
	public class ThemeSpinnerAdapter : BaseAdapter, ISpinnerAdapter {
		public override int Count => 2;
		private readonly string[] THEME_NAMES = {
			"BLUE",
			"VIOLET"
		};
		private readonly string[] THEME_COLORS = {
			"#176DC3",
			"#501460"
		};
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
			CheckedTextView item_view = convert_view as CheckedTextView;
			if (item_view == null) {
				item_view = (CheckedTextView)this._main_activity.LayoutInflater.Inflate(
					Resource.Drawable.theme_spinner_dropdown_item, null);
			}


			item_view.Text = this.THEME_NAMES[position];
			item_view.Background = new ColorDrawable(Color.ParseColor(this.THEME_COLORS[position]));
			return item_view;
		}
	}
}