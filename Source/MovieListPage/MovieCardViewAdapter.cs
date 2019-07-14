using Android.Content;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Square.Picasso;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Movolira {
	internal class MovieCardViewAdapter : RecyclerView.Adapter {
		public override int ItemCount => MovieData.Count();
		public List<MovieCard> MovieData { get; }
		private readonly Context _app_context;

		public event EventHandler<int> click_handler;

		public MovieCardViewAdapter(List<MovieCard> movie_data, Context app_context) {
			MovieData = movie_data;
			_app_context = app_context;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent_view, int view_type) {
			View card_view = LayoutInflater.From(parent_view.Context).Inflate(Resource.Layout.card_movie_item, parent_view, false);
			MovieCardViewHolder holder = new MovieCardViewHolder(card_view, onClick);
			return holder;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
			MovieCardViewHolder card_holder = holder as MovieCardViewHolder;
			card_holder.TitleText.Text = MovieData[position].Title;
			card_holder.GenresText.Text = MovieData[position].Genres;
			double rating = MovieData[position].Rating;
			card_holder.RatingText.Text = $"{rating:F1}";
			if (rating < 3)
				card_holder.RatingText.Background = ContextCompat.GetDrawable(_app_context, Resource.Drawable.card_rating_bad);
			else if (rating < 7)
				card_holder.RatingText.Background = ContextCompat.GetDrawable(_app_context, Resource.Drawable.card_rating_average);
			else
				card_holder.RatingText.Background = ContextCompat.GetDrawable(_app_context, Resource.Drawable.card_rating_good);
			Picasso.With(_app_context).Load(MovieData[position].BackdropSmallUrl).Into(card_holder.BackdropImage);
		}

		private void onClick(int position) {
			click_handler?.Invoke(this, position);
		}
	}
}