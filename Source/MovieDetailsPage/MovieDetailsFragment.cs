using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Resource.Drawable;
using Bumptech.Glide.Request;
using Newtonsoft.Json;
using Orientation = Android.Content.Res.Orientation;

namespace Movolira {
	public class MovieDetailsFragment : Fragment, IBackButtonHandler {
		private MainActivity _main_activity;
		private Movie _movie;

		public override void OnAttach(Context main_activity) {
			_main_activity = (MainActivity) main_activity;
			base.OnAttach(main_activity);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle saved_instance_state) {
			_main_activity.setIsLoading(false);
			View layout = inflater.Inflate(Resource.Layout.movie_details, container, false);
			_movie = JsonConvert.DeserializeObject<Movie>(Arguments.GetString("movie"));
			ImageView backdrop_view = layout.FindViewById<ImageView>(Resource.Id.movie_details_backdrop);
			RequestOptions image_load_options = new RequestOptions().Placeholder(new ColorDrawable(Color.Black)).CenterCrop();
			RequestOptions thumbnail_options = new RequestOptions().CenterCrop();
			if (_main_activity.Resources.Configuration.Orientation == Orientation.Portrait) {
				Glide.With(_main_activity).Load(_movie.BackdropUrl).Transition(DrawableTransitionOptions.WithCrossFade()).Apply(image_load_options)
					.Thumbnail(Glide.With(_main_activity).Load(_movie.BackdropUrl.Replace("/fanart/", "/preview/")).Apply(thumbnail_options)
						.Transition(DrawableTransitionOptions.WithCrossFade())).Into(backdrop_view);
			} else {
				Glide.With(_main_activity).Load(_movie.PosterUrl).Transition(DrawableTransitionOptions.WithCrossFade()).Apply(image_load_options)
					.Thumbnail(Glide.With(_main_activity).Load(_movie.PosterUrl.Replace("/fanart/", "/preview/")).Apply(thumbnail_options)
						.Transition(DrawableTransitionOptions.WithCrossFade())).Into(backdrop_view);
			}
			
			if (_movie.Title != null) {
				TextView title_view = layout.FindViewById<TextView>(Resource.Id.movie_details_title);
				title_view.Text = _movie.Title;
			}
			if (_movie.Genres.Length > 0) {
				TextView genres_view = layout.FindViewById<TextView>(Resource.Id.movie_details_genres);
				genres_view.Text = _movie.Genres[0].First().ToString().ToUpper() + _movie.Genres[0].Substring(1);
				if (_movie.Genres.Length > 1) {
					genres_view.Text += "\n" + _movie.Genres[1].First().ToString().ToUpper() + _movie.Genres[1].Substring(1);
				}
			}
			TextView release_date_view = layout.FindViewById<TextView>(Resource.Id.movie_details_release_date);
			string release_date_title = "Released\n";
			release_date_view.Text = release_date_title;
			if (_movie.ReleaseDate != null) {
				release_date_view.Text += _movie.ReleaseDate;
			} else {
				release_date_view.Text += "-";
			}
			SpannableStringBuilder release_date_styled_string = new SpannableStringBuilder(release_date_view.Text);
			release_date_styled_string.SetSpan(new RelativeSizeSpan(1.2f), release_date_title.Length, release_date_styled_string.Length(),
				SpanTypes.ExclusiveExclusive);
			release_date_view.TextFormatted = release_date_styled_string;
			TextView runtime_view = layout.FindViewById<TextView>(Resource.Id.movie_details_runtime);
			string runtime_title = "Runtime\n";
			runtime_view.Text = runtime_title;
			if (_movie.Runtime > 0) {
				int runtime_hours = _movie.Runtime / 60;
				int runtime_minutes = _movie.Runtime % 60;
				runtime_view.Text += runtime_hours + "h " + runtime_minutes + "min";
			} else {
				runtime_view.Text += "-";
			}
			SpannableStringBuilder runtime_styled_string = new SpannableStringBuilder(runtime_view.Text);
			runtime_styled_string.SetSpan(new RelativeSizeSpan(1.2f), runtime_title.Length, runtime_styled_string.Length(),
				SpanTypes.ExclusiveExclusive);
			runtime_view.TextFormatted = runtime_styled_string;
			TextView certification_view = layout.FindViewById<TextView>(Resource.Id.movie_details_certification);
			string certification_title = "Rated\n";
			certification_view.Text = certification_title;
			if (_movie.Certification != null) {
				certification_view.Text += _movie.Certification;
			} else {
				certification_view.Text += "-";
			}
			SpannableStringBuilder certification_styled_string = new SpannableStringBuilder(certification_view.Text);
			certification_styled_string.SetSpan(new RelativeSizeSpan(1.2f), certification_title.Length, certification_styled_string.Length(),
				SpanTypes.ExclusiveExclusive);
			certification_view.TextFormatted = certification_styled_string;
			int rating = (int) Math.Round(_movie.Rating);
			TextView rating_view = layout.FindViewById<TextView>(Resource.Id.movie_details_rating);
			rating_view.Text = $"{_movie.Rating * 10:F0}%";
			TextView vote_count_view = layout.FindViewById<TextView>(Resource.Id.movie_details_vote_count);
			vote_count_view.Text = _movie.Votes + " votes";
			var rating_stars = new List<ImageView> {
				layout.FindViewById<ImageView>(Resource.Id.movie_details_rating_star_1),
				layout.FindViewById<ImageView>(Resource.Id.movie_details_rating_star_2),
				layout.FindViewById<ImageView>(Resource.Id.movie_details_rating_star_3),
				layout.FindViewById<ImageView>(Resource.Id.movie_details_rating_star_4),
				layout.FindViewById<ImageView>(Resource.Id.movie_details_rating_star_5)
			};
			int i_rating_stars = 0;
			while (rating >= 2) {
				rating_stars[i_rating_stars].SetImageResource(Resource.Mipmap.ic_star_full);
				rating -= 2;
				++i_rating_stars;
			}
			while (rating >= 1) {
				rating_stars[i_rating_stars].SetImageResource(Resource.Mipmap.ic_star_half);
				--rating;
				++i_rating_stars;
			}
			if (_movie.Overview != null) {
				TextView overview_view = layout.FindViewById<TextView>(Resource.Id.movie_details_overview);
				overview_view.Text = _movie.Overview;
			}
			return layout;
		}

		public bool handleBackButtonPress() {
			return false;
		}
	}
}