using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Movolira {
	internal class ShowCardViewHolder : RecyclerView.ViewHolder {
		public ImageView BackdropImage { get; }
		public TextView TitleText { get; }
		public TextView GenresText { get; }
		public TextView RatingText { get; }

		public ShowCardViewHolder(View view, Action<int> click_listener) : base(view) {
			view.Click += (sender, position) => click_listener(LayoutPosition);
			BackdropImage = view.FindViewById<ImageView>(Resource.Id.show_card_backdrop);
			TitleText = view.FindViewById<TextView>(Resource.Id.show_card_title);
			GenresText = view.FindViewById<TextView>(Resource.Id.show_card_genres);
			RatingText = view.FindViewById<TextView>(Resource.Id.show_card_rating);
		}
	}
}