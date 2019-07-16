using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Square.Picasso;

namespace Movolira {
	internal class MovieCardViewAdapter : RecyclerView.Adapter {
		public override int ItemCount => Movies.Count();
		public List<Movie> Movies { get; }
		private readonly MainActivity _main_activity;

		public event EventHandler<int> click_handler;

		public MovieCardViewAdapter(List<Movie> movies, Context main_activity) {
			Movies = movies;
			_main_activity = (MainActivity) main_activity;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent_view, int view_type) {
			View card_view = LayoutInflater.From(parent_view.Context).Inflate(Resource.Layout.card_movie_item, parent_view, false);
			MovieCardViewHolder view_holder = new MovieCardViewHolder(card_view, onClick);
			return view_holder;
		}

		public override async void OnBindViewHolder(RecyclerView.ViewHolder view_holder, int position) {
			MovieCardViewHolder card_holder = view_holder as MovieCardViewHolder;
			Movie movie = Movies[position];
			card_holder.TitleText.Text = movie.Title;
			card_holder.GenresText.Text += movie.Genres[0].First().ToString().ToUpper() + movie.Genres[0].Substring(1);
			for (int i_genre = 1; i_genre < movie.Genres.Length; ++i_genre) {
				card_holder.GenresText.Text += ", " + movie.Genres[i_genre].First().ToString().ToUpper() + movie.Genres[i_genre].Substring(1);
			}
			double rating = movie.Rating;
			card_holder.RatingText.Text = $"{rating:F1}";
			if (rating < 3) {
				card_holder.RatingText.Background = ContextCompat.GetDrawable(_main_activity, Resource.Drawable.card_rating_bad);
			} else if (rating < 7) {
				card_holder.RatingText.Background = ContextCompat.GetDrawable(_main_activity, Resource.Drawable.card_rating_average);
			} else {
				card_holder.RatingText.Background = ContextCompat.GetDrawable(_main_activity, Resource.Drawable.card_rating_good);
			}
			if (movie.BackdropFilePath == null) {
				await Task.Run(() => _main_activity.MovieDataProvider.getMovieImages(movie));
			}
			string backdrop_url = _main_activity.MovieDataProvider.getMovieBackdropUrl(movie.BackdropFilePath);
			Picasso.With(_main_activity).Load(backdrop_url).Into(card_holder.BackdropImage);
		}

		private void onClick(int position) {
			click_handler?.Invoke(this, position);
		}
	}
}