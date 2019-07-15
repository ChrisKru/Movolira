﻿using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;

namespace Movolira {
	internal class MovieCardViewHolder : RecyclerView.ViewHolder {
		public ImageView BackdropImage { get; }
		public TextView TitleText { get; }
		public TextView GenresText { get; }
		public TextView RatingText { get; }

		public MovieCardViewHolder(View view, Action<int> listener) : base(view) {
			view.Click += (sender, position) => listener(LayoutPosition);
			BackdropImage = view.FindViewById<ImageView>(Resource.Id.card_movie_backdrop);
			TitleText = view.FindViewById<TextView>(Resource.Id.card_movie_title);
			GenresText = view.FindViewById<TextView>(Resource.Id.card_movie_genres);
			RatingText = view.FindViewById<TextView>(Resource.Id.card_movie_rating);
		}
	}
}