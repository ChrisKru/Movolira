﻿using System;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Views;
using Bumptech.Glide;
using Bumptech.Glide.Load.Resource.Drawable;
using Bumptech.Glide.Request;

namespace Movolira {
	public class ShowCardViewAdapter : RecyclerView.Adapter {
		public override int ItemCount {
			get {
				if (Shows.Count > 0) {
					return Shows.Count() + 1;
				}
				return 0;
			}
		}

		public List<Movie> Shows { get; set; }
		public int CurrentPageNumber { get; set; }
		public int MaxItemCount { get; set; }
		private readonly MainActivity _main_activity;
		public event EventHandler NextButtonClickEvent;
		public event EventHandler PrevButtonClickEvent;
		public event EventHandler<int> ShowCardClickEvent;

		public ShowCardViewAdapter(List<Movie> shows, MainActivity main_activity) {
			Shows = shows;
			CurrentPageNumber = 1;
			MaxItemCount = 0;
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
				if (CurrentPageNumber == 1) {
					pager_holder.PrevButton.Alpha = 0.5f;
					pager_holder.PrevButton.Enabled = false;
				} else {
					pager_holder.PrevButton.Alpha = 1f;
					pager_holder.PrevButton.Enabled = true;
				}
				if (CurrentPageNumber * DataProvider.SHOWS_PER_PAGE >= MaxItemCount) {
					pager_holder.NextButton.Alpha = 0.5f;
					pager_holder.NextButton.Enabled = false;
				} else {
					pager_holder.NextButton.Alpha = 1f;
					pager_holder.NextButton.Enabled = true;
				}
			} else {
				ShowCardViewHolder card_holder = view_holder as ShowCardViewHolder;
				Movie show = Shows[position];
				RequestOptions image_load_options = new RequestOptions().Placeholder(new ColorDrawable(Color.Black)).CenterCrop();
				RequestOptions thumbnail_options = new RequestOptions().CenterCrop();
				Glide.With(_main_activity).Load(show.PosterUrl).Apply(image_load_options).Transition(DrawableTransitionOptions.WithCrossFade())
					.Thumbnail(Glide.With(_main_activity).Load(show.PosterUrl.Replace("/fanart/", "/preview/")).Apply(thumbnail_options)
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