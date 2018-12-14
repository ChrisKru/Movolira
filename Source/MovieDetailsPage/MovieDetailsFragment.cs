using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Square.Picasso;
using Av4 = Android.Support.V4;
//backdrop 
//poster 
//title 
//genres 
//release date
//rating 
//overview

namespace Movolira {
    public class MovieDetailsFragment : Fragment {
        public override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
        }
        public override void OnAttach(Context activity) {
            main_activity = (MainActivity)activity;
            base.OnAttach(activity);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            View layout = inflater.Inflate(Resource.Layout.movie_details, container, false);
            movie_data = main_activity.tmdb.getMovieDetails(Arguments.GetInt("id"));
            ImageView backdrop = layout.FindViewById<ImageView>(Resource.Id.movie_details_backdrop);
            Picasso.With(Activity).Load(movie_data.backdrop_path).Into(backdrop);
            ImageView poster = layout.FindViewById<ImageView>(Resource.Id.movie_details_poster);
            Picasso.With(Activity).Load(movie_data.poster_path).Into(poster);
            TextView title = layout.FindViewById<TextView>(Resource.Id.movie_details_title);
            title.Text = movie_data.title;
            TextView genres = layout.FindViewById<TextView>(Resource.Id.movie_details_genres);
            genres.Text = movie_data.genres;
            TextView release_date = layout.FindViewById<TextView>(Resource.Id.movie_details_release_date);
            release_date.Text = "Released: " + movie_data.release_date;
            TextView runtime = layout.FindViewById<TextView>(Resource.Id.movie_details_runtime);
            runtime.Text = "Runtime: " + movie_data.runtime;
            double rating = movie_data.rating;
            TextView rating_text = layout.FindViewById<TextView>(Resource.Id.movie_details_rating);
            rating_text.Text = String.Format("{0:F1}", rating);
            TextView rating_outline = layout.FindViewById<TextView>(Resource.Id.movie_details_rating_outline);
            rating_outline.Text = String.Format("{0:F1}", rating);
            rating_outline.Paint.StrokeWidth = 2;
            rating_outline.Paint.SetStyle(Paint.Style.Stroke);
            Shader rating_text_shader;
            Rect rating_text_bounds = new Rect();
            ImageView rating_star = layout.FindViewById<ImageView>(Resource.Id.movie_details_rating_star);
            rating_text.Paint.GetTextBounds(rating_text.Text.ToCharArray(), 0, rating_text.Length(), rating_text_bounds);
            if (rating < 3) {
                rating_text_shader = new LinearGradient(0, 0, rating_text_bounds.Width(), rating_text.LineHeight,
                                     new Color(Av4.Content.ContextCompat.GetColor(main_activity, Resource.Color.rating_bad_gradient_start)),
                                     new Color(Av4.Content.ContextCompat.GetColor(main_activity, Resource.Color.rating_bad_gradient_end)), Shader.TileMode.Clamp);
                rating_star.SetImageResource(Resource.Drawable.rating_star_bad);
            } else if (rating < 7) {
                rating_text_shader = new LinearGradient(0, 0, rating_text_bounds.Width(), rating_text.LineHeight,
                                     new Color(Av4.Content.ContextCompat.GetColor(main_activity, Resource.Color.rating_average_gradient_start)),
                                     new Color(Av4.Content.ContextCompat.GetColor(main_activity, Resource.Color.rating_average_gradient_end)), Shader.TileMode.Clamp);
                rating_star.SetImageResource(Resource.Drawable.rating_star_average);
            } else {
                rating_text_shader = new LinearGradient(0, 0, rating_text_bounds.Width(), rating_text.LineHeight, 
                                     new Color(Av4.Content.ContextCompat.GetColor(main_activity, Resource.Color.rating_good_gradient_start)),
                                     new Color(Av4.Content.ContextCompat.GetColor(main_activity, Resource.Color.rating_good_gradient_end)), Shader.TileMode.Clamp);
                rating_star.SetImageResource(Resource.Drawable.rating_star_good);
            }
            rating_text.Paint.SetShader(rating_text_shader);
            TextView overview = layout.FindViewById<TextView>(Resource.Id.movie_details_overview);
            overview.Text = movie_data.overview;
            return layout;
        }
        DetailedMovie movie_data;
        private MainActivity main_activity;
    }
}