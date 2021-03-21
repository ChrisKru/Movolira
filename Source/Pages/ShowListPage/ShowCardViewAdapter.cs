using System;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Views;
using Bumptech.Glide;
using Bumptech.Glide.Load.Resource.Drawable;
using Bumptech.Glide.Request;

namespace Movolira.Pages.ShowListPage {
	public class ShowCardViewAdapter : RecyclerView.Adapter {
		public override int ItemCount {
			get {
				// Besides the shows, there's a single view that holds the 2 buttons at the bottom,
				// the "PREV" and "NEXT" navigation buttons.
				// Which is why there's a "+1" added to the ItemCount getter.
				// When show count is 0, the navigation buttons don't appear, which is why it ends up returning 0.
				if (this.Shows.Count > 0) {
					return this.Shows.Count + 1;
				}
				return 0;
			}
		}


		public event EventHandler NextButtonClickEvent;
		public event EventHandler PrevButtonClickEvent;
		public event EventHandler<int> ShowCardClickEvent;


		public List<Show> Shows { get; set; }
		public int CurrentPageNumber { get; set; }
		public int MaxItemCount { get; set; }
		private MainActivity _main_activity;





		public ShowCardViewAdapter(List<Show> shows, MainActivity main_activity) {
			this.Shows = shows;
			this.CurrentPageNumber = 1;
			this.MaxItemCount = 0;
			this._main_activity = main_activity;
		}




		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent_view, int view_type) {
			if (view_type == Resource.Layout.show_list_pager) {
				View pager_view = LayoutInflater.From(parent_view.Context)
					.Inflate(Resource.Layout.show_list_pager, parent_view, false);
				ShowListPagerHolder pager_holder = new ShowListPagerHolder(
					pager_view, this.onNextButtonClick, this.onPrevButtonClick);
				return pager_holder;


			} else {
				View card_view = LayoutInflater.From(parent_view.Context)
					.Inflate(Resource.Layout.show_card, parent_view, false);
				ShowCardViewHolder card_holder = new ShowCardViewHolder(card_view, this.onCardClick);
				return card_holder;
			}
		}




		public override void OnBindViewHolder(RecyclerView.ViewHolder view_holder, int view_index) {
			if (view_index == this.Shows.Count) {
				this.onBindPagerViewHolder(view_holder);
			} else {
				this.onBindCardViewHolder(view_holder, view_index);
			}
		}




		private void onBindPagerViewHolder(RecyclerView.ViewHolder view_holder) {
			ShowListPagerHolder pager_holder = view_holder as ShowListPagerHolder;


			if (this.CurrentPageNumber == 1) {
				pager_holder.PrevButton.Alpha = 0.5f;
				pager_holder.PrevButton.Enabled = false;
			} else {
				pager_holder.PrevButton.Alpha = 1f;
				pager_holder.PrevButton.Enabled = true;
			}


			if (this.CurrentPageNumber * MainActivity.SHOWS_PER_PAGE >= this.MaxItemCount) {
				pager_holder.NextButton.Alpha = 0.5f;
				pager_holder.NextButton.Enabled = false;
			} else {
				pager_holder.NextButton.Alpha = 1f;
				pager_holder.NextButton.Enabled = true;
			}


			if (!pager_holder.NextButton.Enabled && !pager_holder.PrevButton.Enabled) {
				pager_holder.NextButton.Visibility = ViewStates.Gone;
				pager_holder.PrevButton.Visibility = ViewStates.Gone;
			} else {
				pager_holder.NextButton.Visibility = ViewStates.Visible;
				pager_holder.PrevButton.Visibility = ViewStates.Visible;
			}
		}




		private void onBindCardViewHolder(RecyclerView.ViewHolder view_holder, int view_index) {
			ShowCardViewHolder card_holder = view_holder as ShowCardViewHolder;
			Show show = this.Shows[view_index];


			RequestOptions image_load_options = new RequestOptions().CenterCrop()
				.Placeholder(new ColorDrawable(Color.Black))
				.Error(new ColorDrawable(Color.LightGray));
			RequestOptions thumbnail_options = new RequestOptions().CenterCrop();
			Glide.With(this._main_activity).Load(show.PosterUrl).Apply(image_load_options)
				.Transition(DrawableTransitionOptions.WithCrossFade())
				.Thumbnail(Glide.With(this._main_activity).Load(show.PosterUrl.Replace("/fanart/", "/preview/"))
				.Apply(thumbnail_options).Transition(DrawableTransitionOptions.WithCrossFade()))
				.Into(card_holder.BackdropImage);


			if (show.Title != null) {
				card_holder.TitleText.Text = show.Title;
			}
			if (show.Genres.Length > 0) {
				// Capitalizes first letter.
				card_holder.GenresText.Text = show.Genres[0].First().ToString().ToUpper()
					+ show.Genres[0].Substring(1);
				if (show.Genres.Length > 1) {
					card_holder.GenresText.Text += " " + show.Genres[1].First().ToString().ToUpper()
						+ show.Genres[1].Substring(1);
				}
			}
		}




		public override int GetItemViewType(int position) {
			if (position == this.Shows.Count()) {
				return Resource.Layout.show_list_pager;
			}
			return Resource.Layout.show_card;
		}




		private void onCardClick(int card_index) {
			ShowCardClickEvent?.Invoke(this, card_index);
		}




		private void onNextButtonClick() {
			NextButtonClickEvent?.Invoke(this, EventArgs.Empty);
		}




		private void onPrevButtonClick() {
			PrevButtonClickEvent?.Invoke(this, EventArgs.Empty);
		}
	}
}