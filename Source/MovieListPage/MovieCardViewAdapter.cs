using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Load.Resource.Drawable;

namespace Movolira {
	internal class MovieCardViewAdapter : RecyclerView.Adapter {
		public override int ItemCount {
			get {
				if (Movies.Count > 0) {
					return Movies.Count() + 1;
				}
				return 0;
			}
		}

		public List<Movie> Movies { get; set; }
		private readonly MainActivity _main_activity;

		public event EventHandler<int> MovieCardClickEvent;
		public event EventHandler NextButtonClickEvent;
		public event EventHandler PrevButtonClickEvent;

		public MovieCardViewAdapter(List<Movie> movies, Context main_activity) {
			Movies = movies;
			_main_activity = (MainActivity) main_activity;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent_view, int view_type) {
			if (view_type == Resource.Layout.movie_list_pager) {
				View pager = LayoutInflater.From(parent_view.Context).Inflate(Resource.Layout.movie_list_pager, parent_view, false);
				MovieListPagerHolder pager_holder = new MovieListPagerHolder(pager, onNextButtonClick, onPrevButtonClick);
				return pager_holder;
			}
			View card_view = LayoutInflater.From(parent_view.Context).Inflate(Resource.Layout.movie_card, parent_view, false);
			MovieCardViewHolder view_holder = new MovieCardViewHolder(card_view, onCardClick);
			return view_holder;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder view_holder, int position) {
			if (position == Movies.Count()) { } else {
				MovieCardViewHolder card_holder = view_holder as MovieCardViewHolder;
				Movie movie = Movies[position];
				card_holder.TitleText.Text = movie.Title;
				card_holder.GenresText.Text = movie.Genres[0].First().ToString().ToUpper() + movie.Genres[0].Substring(1);
				if (movie.Genres.Length > 1) {
					card_holder.GenresText.Text += " " + movie.Genres[1].First().ToString().ToUpper() + movie.Genres[1].Substring(1);
				}
				double rating = movie.Rating;
				card_holder.RatingText.Text = $"{rating * 10:F0}%";
				if (rating < 3) {
					card_holder.RatingText.Background = ContextCompat.GetDrawable(_main_activity, Resource.Drawable.card_rating_bad);
				} else if (rating < 7) {
					card_holder.RatingText.Background = ContextCompat.GetDrawable(_main_activity, Resource.Drawable.card_rating_average);
				} else {
					card_holder.RatingText.Background = ContextCompat.GetDrawable(_main_activity, Resource.Drawable.card_rating_good);
				}
				Glide.With(_main_activity).Load(movie.PosterUrl).Transition(DrawableTransitionOptions.WithCrossFade())
					.Into(card_holder.BackdropImage);
			}
		}

		public override int GetItemViewType(int position) {
			if (position == Movies.Count()) {
				return Resource.Layout.movie_list_pager;
			}
			return Resource.Layout.movie_card;
		}

		private void onCardClick(int position) {
			MovieCardClickEvent?.Invoke(this, position);
		}

		private void onNextButtonClick() {
			NextButtonClickEvent?.Invoke(this, EventArgs.Empty);
		}

		private void onPrevButtonClick() {
			PrevButtonClickEvent?.Invoke(this, EventArgs.Empty);
		}
	}
}