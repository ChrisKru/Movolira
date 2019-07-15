using Android.Content;
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
		private DetailedMovie _detailed_movie;

		public override void OnAttach(Context activity) {
			_main_activity = (MainActivity) activity;
			base.OnAttach(activity);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			View layout = inflater.Inflate(Resource.Layout.movie_details, container, false);
			_detailed_movie = _main_activity.MovieDataProvider.getDetailedMovie(Arguments.GetInt("id"));
			ImageView backdrop_view = layout.FindViewById<ImageView>(Resource.Id.movie_details_backdrop);
			Picasso.With(Activity).Load(_detailed_movie.BackdropUrl).Into(backdrop_view);
			ImageView poster_view = layout.FindViewById<ImageView>(Resource.Id.movie_details_poster);
			Picasso.With(Activity).Load(_detailed_movie.PosterUrl).Into(poster_view);
			TextView title_view = layout.FindViewById<TextView>(Resource.Id.movie_details_title);
			title_view.Text = _detailed_movie.Title;
			TextView genres_view = layout.FindViewById<TextView>(Resource.Id.movie_details_genres);
			genres_view.Text = _detailed_movie.Genres;
			TextView release_date_view = layout.FindViewById<TextView>(Resource.Id.movie_details_release_date);
			release_date_view.Text = "Released: " + _detailed_movie.ReleaseDate;
			TextView runtime_view = layout.FindViewById<TextView>(Resource.Id.movie_details_runtime);
			runtime_view.Text = "Runtime: " + _detailed_movie.Runtime;
			double rating = _detailed_movie.Rating;
			TextView rating_view = layout.FindViewById<TextView>(Resource.Id.movie_details_rating);
			rating_view.Text = $"{rating:F1}";
			TextView rating_outline = layout.FindViewById<TextView>(Resource.Id.movie_details_rating_outline);
			rating_outline.Text = $"{rating:F1}";
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
			}
			else if (rating < 7) {
				rating_text_shader = new LinearGradient(0, 0, rating_text_bounds.Width(), rating_view.LineHeight,
					new Color(ContextCompat.GetColor(_main_activity, Resource.Color.rating_average_gradient_start)),
					new Color(ContextCompat.GetColor(_main_activity, Resource.Color.rating_average_gradient_end)), Shader.TileMode.Clamp);
				rating_star.SetImageResource(Resource.Drawable.rating_star_average);
			}
			else {
				rating_text_shader = new LinearGradient(0, 0, rating_text_bounds.Width(), rating_view.LineHeight,
					new Color(ContextCompat.GetColor(_main_activity, Resource.Color.rating_good_gradient_start)),
					new Color(ContextCompat.GetColor(_main_activity, Resource.Color.rating_good_gradient_end)), Shader.TileMode.Clamp);
				rating_star.SetImageResource(Resource.Drawable.rating_star_good);
			}

			rating_view.Paint.SetShader(rating_text_shader);
			TextView overview_view = layout.FindViewById<TextView>(Resource.Id.movie_details_overview);
			overview_view.Text = _detailed_movie.Overview;
			return layout;
		}
	}
}