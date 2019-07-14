﻿using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Square.Picasso;

namespace Movolira {
	public class MovieDetailsFragment : Fragment {
		private MainActivity _main_activity;
		private DetailedMovie _movie_data;

		public override void OnAttach(Context activity) {
			_main_activity = (MainActivity) activity;
			base.OnAttach(activity);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			View layout = inflater.Inflate(Resource.Layout.movie_details, container, false);
			_movie_data = _main_activity.MovieDataProvider.getMovieDetails(Arguments.GetInt("id"));
			ImageView backdrop = layout.FindViewById<ImageView>(Resource.Id.movie_details_backdrop);
			Picasso.With(Activity).Load(_movie_data.BackdropPath).Into(backdrop);
			ImageView poster = layout.FindViewById<ImageView>(Resource.Id.movie_details_poster);
			Picasso.With(Activity).Load(_movie_data.PosterPath).Into(poster);
			TextView title = layout.FindViewById<TextView>(Resource.Id.movie_details_title);
			title.Text = _movie_data.Title;
			TextView genres = layout.FindViewById<TextView>(Resource.Id.movie_details_genres);
			genres.Text = _movie_data.Genres;
			TextView release_date = layout.FindViewById<TextView>(Resource.Id.movie_details_release_date);
			release_date.Text = "Released: " + _movie_data.ReleaseDate;
			TextView runtime = layout.FindViewById<TextView>(Resource.Id.movie_details_runtime);
			runtime.Text = "Runtime: " + _movie_data.Runtime;
			double rating = _movie_data.Rating;
			TextView rating_text = layout.FindViewById<TextView>(Resource.Id.movie_details_rating);
			rating_text.Text = $"{rating:F1}";
			TextView rating_outline = layout.FindViewById<TextView>(Resource.Id.movie_details_rating_outline);
			rating_outline.Text = $"{rating:F1}";
			rating_outline.Paint.StrokeWidth = 2;
			rating_outline.Paint.SetStyle(Paint.Style.Stroke);
			Shader rating_text_shader;
			Rect rating_text_bounds = new Rect();
			ImageView rating_star = layout.FindViewById<ImageView>(Resource.Id.movie_details_rating_star);
			rating_text.Paint.GetTextBounds(rating_text.Text.ToCharArray(), 0, rating_text.Length(), rating_text_bounds);
			if (rating < 3) {
				rating_text_shader = new LinearGradient(0, 0, rating_text_bounds.Width(), rating_text.LineHeight,
					new Color(ContextCompat.GetColor(_main_activity, Resource.Color.rating_bad_gradient_start)),
					new Color(ContextCompat.GetColor(_main_activity, Resource.Color.rating_bad_gradient_end)), Shader.TileMode.Clamp);
				rating_star.SetImageResource(Resource.Drawable.rating_star_bad);
			}
			else if (rating < 7) {
				rating_text_shader = new LinearGradient(0, 0, rating_text_bounds.Width(), rating_text.LineHeight,
					new Color(ContextCompat.GetColor(_main_activity, Resource.Color.rating_average_gradient_start)),
					new Color(ContextCompat.GetColor(_main_activity, Resource.Color.rating_average_gradient_end)), Shader.TileMode.Clamp);
				rating_star.SetImageResource(Resource.Drawable.rating_star_average);
			}
			else {
				rating_text_shader = new LinearGradient(0, 0, rating_text_bounds.Width(), rating_text.LineHeight,
					new Color(ContextCompat.GetColor(_main_activity, Resource.Color.rating_good_gradient_start)),
					new Color(ContextCompat.GetColor(_main_activity, Resource.Color.rating_good_gradient_end)), Shader.TileMode.Clamp);
				rating_star.SetImageResource(Resource.Drawable.rating_star_good);
			}

			rating_text.Paint.SetShader(rating_text_shader);
			TextView overview = layout.FindViewById<TextView>(Resource.Id.movie_details_overview);
			overview.Text = _movie_data.Overview;
			return layout;
		}
	}
}