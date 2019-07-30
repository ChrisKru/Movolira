using System;
using System.Collections.Generic;
using System.Linq;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Bumptech.Glide;
using Bumptech.Glide.Load.Resource.Drawable;

namespace Movolira {
	internal class ShowCardViewAdapter : RecyclerView.Adapter {
		public override int ItemCount {
			get {
				if (Shows.Count > 0) {
					return Shows.Count() + 1;
				}
				return 0;
			}
		}

		public List<Movie> Shows { get; set; }
		public int CurrentPage { get; set; }
		private readonly MainActivity _main_activity;
		public event EventHandler NextButtonClickEvent;
		public event EventHandler PrevButtonClickEvent;
		public event EventHandler<int> ShowCardClickEvent;

		public ShowCardViewAdapter(List<Movie> shows, MainActivity main_activity) {
			Shows = shows;
			CurrentPage = 1;
			_main_activity = main_activity;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent_view, int view_type) {
			if (view_type == Resource.Layout.show_list_pager) {
				View pager = LayoutInflater.From(parent_view.Context).Inflate(Resource.Layout.show_list_pager, parent_view, false);
				ShowListPagerHolder pager_holder = new ShowListPagerHolder(pager, onNextButtonClick, onPrevButtonClick);
				return pager_holder;
			}
			View card_view = LayoutInflater.From(parent_view.Context).Inflate(Resource.Layout.show_card, parent_view, false);
			ShowCardViewHolder card_holder = new ShowCardViewHolder(card_view, onCardClick);
			return card_holder;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder view_holder, int position) {
			if (position == Shows.Count) {
				ShowListPagerHolder pager_holder = view_holder as ShowListPagerHolder;
				if (CurrentPage == 1) {
					pager_holder.PrevButton.Visibility = ViewStates.Gone;
				} else {
					pager_holder.PrevButton.Visibility = ViewStates.Visible;
				}
			} else {
				ShowCardViewHolder card_holder = view_holder as ShowCardViewHolder;
				Movie show = Shows[position];
				Glide.With(_main_activity).Load(show.PosterUrl)
					.Thumbnail(Glide.With(_main_activity).Load(show.PosterUrl.Replace("/fanart/", "/preview/"))
						.Transition(DrawableTransitionOptions.WithCrossFade())).Into(card_holder.BackdropImage);
				if (show.Title != null) {
					card_holder.TitleText.Text = show.Title;
				}
				if (show.Genres.Length > 0) {
					card_holder.GenresText.Text = show.Genres[0].First().ToString().ToUpper() + show.Genres[0].Substring(1);
					if (show.Genres.Length > 1) {
						card_holder.GenresText.Text += " " + show.Genres[1].First().ToString().ToUpper() + show.Genres[1].Substring(1);
					}
				}
				double rating = show.Rating;
				card_holder.RatingText.Text = $"{rating * 10:F0}%";
				if (rating < 3) {
					card_holder.RatingText.Background = ContextCompat.GetDrawable(_main_activity, Resource.Drawable.card_rating_bad);
				} else if (rating < 7) {
					card_holder.RatingText.Background = ContextCompat.GetDrawable(_main_activity, Resource.Drawable.card_rating_average);
				} else {
					card_holder.RatingText.Background = ContextCompat.GetDrawable(_main_activity, Resource.Drawable.card_rating_good);
				}
			}
		}

		public override int GetItemViewType(int position) {
			if (position == Shows.Count()) {
				return Resource.Layout.show_list_pager;
			}
			return Resource.Layout.show_card;
		}

		private void onCardClick(int position) {
			ShowCardClickEvent?.Invoke(this, position);
		}

		private void onNextButtonClick() {
			NextButtonClickEvent?.Invoke(this, EventArgs.Empty);
		}

		private void onPrevButtonClick() {
			PrevButtonClickEvent?.Invoke(this, EventArgs.Empty);
		}
	}
}