using System;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;




namespace Movolira.Pages.ShowListPage {
	public class ShowListPagerHolder : RecyclerView.ViewHolder {
		public Button NextButton { get; }
		public Button PrevButton { get; }




		public ShowListPagerHolder(View view, Action next_click_listener, Action prev_click_listener) : base(view) {
			this.NextButton = view.FindViewById<Button>(Resource.Id.show_list_next_button);
			this.NextButton.Click += (sender, args) => next_click_listener();
			this.PrevButton = view.FindViewById<Button>(Resource.Id.show_list_prev_button);
			this.PrevButton.Click += (sender, args) => prev_click_listener();
		}
	}
}