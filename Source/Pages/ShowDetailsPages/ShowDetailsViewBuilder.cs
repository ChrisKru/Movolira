using System;
using System.Linq;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Resource.Drawable;
using Bumptech.Glide.Request;
using Orientation = Android.Content.Res.Orientation;




namespace Movolira.Pages.ShowDetailsPages {
	public static class ShowDetailsViewBuilder {
		public static void buildBackdropView(MainActivity main_activity, View layout, Show show, int backdrop_view_resource_id) {
			ImageView backdrop_view = layout.FindViewById<ImageView>(backdrop_view_resource_id);
			RequestOptions image_load_options = new RequestOptions().CenterCrop().Placeholder(new ColorDrawable(Color.Black))
				.Error(Color.LightGray);
			RequestOptions thumbnail_options = new RequestOptions().CenterCrop();


			// In landscape orientation, the backdrop view is on the side, which is why the poster url is used instead.
			string backdrop_view_image_url;
			if (main_activity.Resources.Configuration.Orientation == Orientation.Portrait) {
				backdrop_view_image_url = show.BackdropUrl;
			} else {
				backdrop_view_image_url = show.PosterUrl;
			}


			Glide.With(main_activity).Load(backdrop_view_image_url)
					.Transition(DrawableTransitionOptions.WithCrossFade()).Apply(image_load_options)
					.Thumbnail(Glide.With(main_activity).Load(backdrop_view_image_url.Replace("/fanart/", "/preview/"))
					.Apply(thumbnail_options).Transition(DrawableTransitionOptions.WithCrossFade())).Into(backdrop_view);
		}




		public static void buildTitleView(MainActivity main_activity, View layout, Show show, int title_view_resource_id) {
			if (show.Title != null) {
				TextView title_view = layout.FindViewById<TextView>(title_view_resource_id);
				title_view.Text = show.Title;
				main_activity.setToolbarTitle(show.Title);


			} else {
				main_activity.setToolbarTitle("Movolira");
			}
		}




		public static void buildGenresView(View layout, Show show, int genres_view_resource_id) {
			if (show.Genres.Length > 0) {
				TextView genres_view = layout.FindViewById<TextView>(genres_view_resource_id);
				// Capitalizes first letter.
				genres_view.Text = show.Genres[0].First().ToString().ToUpper() + show.Genres[0].Substring(1);


				if (show.Genres.Length > 1) {
					genres_view.Text += "\n" + show.Genres[1].First().ToString().ToUpper()
						+ show.Genres[1].Substring(1);
				}
			}
		}




		public static void buildCertificationView(View layout, Show show, int certification_view_resource_id) {
			TextView certification_view = layout.FindViewById<TextView>(certification_view_resource_id);
			string certification_title = "Certified\n";
			certification_view.Text = certification_title;


			if (show.Certification != null) {
				certification_view.Text += show.Certification;
			} else {
				certification_view.Text += "-";
			}


			// Enlarges the number value.
			SpannableStringBuilder certification_styled_string = new SpannableStringBuilder(certification_view.Text);
			certification_styled_string.SetSpan(new RelativeSizeSpan(1.2f), certification_title.Length,
				certification_styled_string.Length(), SpanTypes.ExclusiveExclusive);
			certification_view.TextFormatted = certification_styled_string;
		}




		public static void buildRatingView(View layout, Show show, int rating_view_resource_id, int vote_count_view_resource_id,
			int[] rating_star_view_resource_ids) {


			TextView rating_view = layout.FindViewById<TextView>(rating_view_resource_id);
			rating_view.Text = $"{show.Rating * 10:F0}%";
			TextView vote_count_view = layout.FindViewById<TextView>(vote_count_view_resource_id);
			vote_count_view.Text = show.Votes + " votes";


			layout.FindViewById<ImageView>(rating_star_view_resource_ids[0])
				.SetImageResource(Resource.Drawable.ic_star_crop_full);
			int rating = (int)Math.Floor(show.Rating);


			for (int i_rating = 2; i_rating < 10; i_rating += 2) {
				if (rating >= i_rating + 1) {
					layout.FindViewById<ImageView>(rating_star_view_resource_ids[i_rating / 2])
						.SetImageResource(Resource.Drawable.ic_star_crop_full);
				} else if (rating >= i_rating) {
					layout.FindViewById<ImageView>(rating_star_view_resource_ids[i_rating / 2])
						.SetImageResource(Resource.Drawable.ic_star_crop_half);
				} else {
					break;
				}
			}
		}




		public static void buildOverviewView(View layout, Show show, int overview_view_resource_id) {
			if (show.Overview != null) {
				TextView overview_view = layout.FindViewById<TextView>(overview_view_resource_id);
				overview_view.Text = show.Overview;
			}
		}




		public static void buildWatchlistButton(MainActivity main_activity, View layout, Show show, int watchlist_button_resource_id) {
			Button watchlist_button = layout.FindViewById<Button>(watchlist_button_resource_id);
			watchlist_button.SetOnClickListener(new WatchlistButtonClickListener(main_activity,
				watchlist_button, show));
		}




		public static void buildRatingButton(MainActivity main_activity, View layout, Show show, int rating_button_resource_id) {
			Button rating_button = layout.FindViewById<Button>(rating_button_resource_id);
			rating_button.SetOnClickListener(new RatingButtonClickListener(main_activity,
				rating_button, show));
		}
	}
}