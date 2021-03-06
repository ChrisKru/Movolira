using System;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;




namespace Movolira.Pages.ShowListPage {
	public class ShowCardViewHolder : RecyclerView.ViewHolder {
		public ImageView BackdropImage { get; }
		public TextView TitleText { get; }
		public TextView GenresText { get; }




		public ShowCardViewHolder(View view, Action<int> click_listener) : base(view) {
			view.Click += (sender, position) => click_listener(this.LayoutPosition);
			this.BackdropImage = view.FindViewById<ImageView>(Resource.Id.show_card_backdrop);
			this.TitleText = view.FindViewById<TextView>(Resource.Id.show_card_title);
			this.GenresText = view.FindViewById<TextView>(Resource.Id.show_card_genres);
		}
	}
}