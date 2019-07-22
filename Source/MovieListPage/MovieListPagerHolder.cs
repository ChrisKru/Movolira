using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Movolira {
	internal class MovieListPagerHolder : RecyclerView.ViewHolder {
		public Button NextButton { get; }
		public Button PrevButton { get; }

		public MovieListPagerHolder(View view, Action next_click_listener, Action prev_click_listener) : base(view) {
			NextButton = view.FindViewById<Button>(Resource.Id.movie_list_next_button);
			NextButton.Click += (sender, args) => next_click_listener();
			PrevButton = view.FindViewById<Button>(Resource.Id.movie_list_prev_button);
			PrevButton.Click += (sender, args) => prev_click_listener();
		}
	}
}