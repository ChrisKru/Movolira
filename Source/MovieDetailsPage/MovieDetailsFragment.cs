using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Resource.Drawable;
using Newtonsoft.Json;

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
			ImageView poster_view = layout.FindViewById<ImageView>(Resource.Id.movie_details_poster);
			Glide.With(_main_activity).Load(_movie.PosterUrl).Thumbnail(Glide.With(_main_activity)
				.Load(_movie.PosterUrl.Replace("/fanart/", "/preview/")).Transition(DrawableTransitionOptions.WithCrossFade())).Into(poster_view);
			Glide.With(_main_activity).Load(_movie.BackdropUrl).Thumbnail(Glide.With(_main_activity)
				.Load(_movie.BackdropUrl.Replace("/fanart/", "/preview/")).Transition(DrawableTransitionOptions.WithCrossFade())).Into(backdrop_view);
			if (_movie.Title != null) {
				TextView title_view = layout.FindViewById<TextView>(Resource.Id.movie_details_title);
				title_view.Text = _movie.Title;
			}
			if (_movie.Genres.Length > 0) {
				TextView genres_view = layout.FindViewById<TextView>(Resource.Id.movie_details_genres);
				genres_view.Text = _movie.Genres[0].First().ToString().ToUpper() + _movie.Genres[0].Substring(1);
				if (_movie.Genres.Length > 1) {
					genres_view.Text += " " + _movie.Genres[1].First().ToString().ToUpper() + _movie.Genres[1].Substring(1);
				}
			}
			TextView release_date_view = layout.FindViewById<TextView>(Resource.Id.movie_details_release_date);
			if (_movie.ReleaseDate != null) {
				release_date_view.Text = _movie.ReleaseDate;
			} else {
				release_date_view.Text = "Unknown";
			}
			TextView runtime_view = layout.FindViewById<TextView>(Resource.Id.movie_details_runtime);
			if (_movie.Runtime > 0) {
				int runtime_hours = _movie.Runtime / 60;
				int runtime_minutes = _movie.Runtime % 60;
				runtime_view.Text = runtime_hours + "h " + runtime_minutes + "min";
			} else {
				runtime_view.Text = "Unknown";
			}
			TextView certification_view = layout.FindViewById<TextView>(Resource.Id.movie_details_certification);
			if (_movie.Certification != null) {
				certification_view.Text = _movie.Certification;
			} else {
				certification_view.Text = "Unknown";
			}

			double rating = _movie.Rating;
			TextView rating_view = layout.FindViewById<TextView>(Resource.Id.movie_details_rating);
			rating_view.Text = $"{rating * 10:F0}%";
			TextView rating_outline = layout.FindViewById<TextView>(Resource.Id.movie_details_rating_outline);
			rating_outline.Text = $"{rating * 10:F0}%";
			rating_outline.Paint.StrokeWidth = 2;
			rating_outline.Paint.SetStyle(Paint.Style.Stroke);
			Shader rating_text_shader;
			Rect rating_text_bounds = new Rect();
			ImageView rating_star = layout.FindViewById<ImageView>(Resource.Id.movie_details_rating_star);
			rating_view.Paint.GetTextBounds(rating_view.Text.ToCharArray(), 0, rating_view.Length(), rating_text_bounds);
			if (rating < 3) {
				rating_text_shader = new LinearGradient(0, 0, rating_text_bounds.Width(), rating_view.LineHeight,
					new Color(ContextCompat.GetColor(_main_activity, Resource.Color.rating_bad_gradient_start)),
					new Color(ContextCompat.GetColor(_main_activity, Resource.Color.rating_bad_gradient_end)), Shader.TileMode.Clamp);
				rating_star.SetImageResource(Resource.Drawable.rating_star_bad);
			} else if (rating < 7) {
				rating_text_shader = new LinearGradient(0, 0, rating_text_bounds.Width(), rating_view.LineHeight,
					new Color(ContextCompat.GetColor(_main_activity, Resource.Color.rating_average_gradient_start)),
					new Color(ContextCompat.GetColor(_main_activity, Resource.Color.rating_average_gradient_end)), Shader.TileMode.Clamp);
				rating_star.SetImageResource(Resource.Drawable.rating_star_average);
			} else {
				rating_text_shader = new LinearGradient(0, 0, rating_text_bounds.Width(), rating_view.LineHeight,
					new Color(ContextCompat.GetColor(_main_activity, Resource.Color.rating_good_gradient_start)),
					new Color(ContextCompat.GetColor(_main_activity, Resource.Color.rating_good_gradient_end)), Shader.TileMode.Clamp);
				rating_star.SetImageResource(Resource.Drawable.rating_star_good);
			}
			rating_view.Paint.SetShader(rating_text_shader);
			TextView vote_count_view = layout.FindViewById<TextView>(Resource.Id.movie_details_vote_count);
			vote_count_view.Text = _movie.Votes + " votes";

			if (_movie.Overview != null) {
				TextView overview_view = layout.FindViewById<TextView>(Resource.Id.movie_details_overview);
				overview_view.Text = _movie.Overview;
				overview_view.Visibility = ViewStates.Visible;
				TextView overview_unconstrained = layout.FindViewById<TextView>(Resource.Id.movie_details_overview_unconstrained);
				overview_unconstrained.Text = "";
			}

			layout.ViewTreeObserver.AddOnGlobalLayoutListener(new OverviewViewSpanModifier(layout));
			return layout;
		}

		public bool handleBackButtonPress() {
			return false;
		}
	}
}