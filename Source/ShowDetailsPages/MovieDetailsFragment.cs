using System;
using System.Linq;
using System.Threading.Tasks;
using Android.Animation;
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
	public class MovieDetailsFragment : Fragment {
		private MainActivity _main_activity;
		private Movie _movie;




		public override void OnAttach(Context main_activity) {
			_main_activity = (MainActivity) main_activity;
			base.OnAttach(main_activity);
		}




		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle saved_instance_state) {
			View layout = inflater.Inflate(Resource.Layout.movie_details_page, container, false);
			LayoutTransition layout_transition = new LayoutTransition();
			layout.FindViewById<ViewGroup>(Resource.Id.movie_details_content).LayoutTransition = layout_transition;
			_movie = JsonConvert.DeserializeObject<Movie>(Arguments.GetString("movie"));


			Task.Run(() => buildMovieData(layout));
			return layout;
		}




		private async void buildMovieData(View layout) {
			await _main_activity.DataProvider.getMovieDetails(_movie);


			_main_activity.RunOnUiThread(() => {
				buildBackdropView(layout);
				buildTitleView(layout);
				buildGenresView(layout);
				buildReleaseDateView(layout);
				buildRuntimeView(layout);
				buildCertificationView(layout);
				buildRatingView(layout);
				buildOverviewView(layout);
				buildWatchlistButton(layout);
				buildRatingButton(layout);
				layout.FindViewById<View>(Resource.Id.movie_details_info).Visibility = ViewStates.Visible;
				_main_activity.setIsLoading(false);
			});
		}




		private void buildBackdropView(View layout) {
			ImageView backdrop_view = layout.FindViewById<ImageView>(Resource.Id.movie_details_backdrop);
			RequestOptions image_load_options = new RequestOptions().CenterCrop().Placeholder(new ColorDrawable(Color.Black))
				.Error(new ColorDrawable(Color.LightGray));
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
		}




		private void buildTitleView(View layout) {
			if (_movie.Title != null) {
				TextView title_view = layout.FindViewById<TextView>(Resource.Id.movie_details_title);
				title_view.Text = _movie.Title;
				_main_activity.setToolbarTitle(_movie.Title);
			} else {
				_main_activity.setToolbarTitle("Movolira");
			}
		}




		private void buildGenresView(View layout) {
			if (_movie.Genres.Length > 0) {
				TextView genres_view = layout.FindViewById<TextView>(Resource.Id.movie_details_genres);
				genres_view.Text = _movie.Genres[0].First().ToString().ToUpper() + _movie.Genres[0].Substring(1);
				if (_movie.Genres.Length > 1) {
					genres_view.Text += "\n" + _movie.Genres[1].First().ToString().ToUpper() + _movie.Genres[1].Substring(1);
				}
			}
		}




		private void buildReleaseDateView(View layout) {
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
		}




		private void buildRuntimeView(View layout) {
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
		}




		private void buildCertificationView(View layout) {
			TextView certification_view = layout.FindViewById<TextView>(Resource.Id.movie_details_certification);
			string certification_title = "Certified\n";
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
		}




		private void buildRatingView(View layout) {
			TextView rating_view = layout.FindViewById<TextView>(Resource.Id.movie_details_rating);
			rating_view.Text = $"{_movie.Rating * 10:F0}%";


			TextView vote_count_view = layout.FindViewById<TextView>(Resource.Id.movie_details_vote_count);
			vote_count_view.Text = _movie.Votes + " votes";


			layout.FindViewById<ImageView>(Resource.Id.movie_details_rating_star_1).SetImageResource(Resource.Drawable.ic_star_crop_full);
			int rating = (int) Math.Floor(_movie.Rating);


			if (rating >= 3) {
				layout.FindViewById<ImageView>(Resource.Id.movie_details_rating_star_2).SetImageResource(Resource.Drawable.ic_star_crop_full);
			} else if (rating >= 2) {
				layout.FindViewById<ImageView>(Resource.Id.movie_details_rating_star_2).SetImageResource(Resource.Drawable.ic_star_crop_half);
			}


			if (rating >= 5) {
				layout.FindViewById<ImageView>(Resource.Id.movie_details_rating_star_3).SetImageResource(Resource.Drawable.ic_star_crop_full);
			} else if (rating >= 4) {
				layout.FindViewById<ImageView>(Resource.Id.movie_details_rating_star_3).SetImageResource(Resource.Drawable.ic_star_crop_half);
			}


			if (rating >= 7) {
				layout.FindViewById<ImageView>(Resource.Id.movie_details_rating_star_4).SetImageResource(Resource.Drawable.ic_star_crop_full);
			} else if (rating >= 6) {
				layout.FindViewById<ImageView>(Resource.Id.movie_details_rating_star_4).SetImageResource(Resource.Drawable.ic_star_crop_half);
			}

			if (rating >= 9) {
				layout.FindViewById<ImageView>(Resource.Id.movie_details_rating_star_5).SetImageResource(Resource.Drawable.ic_star_crop_full);
			} else if (rating >= 8) {
				layout.FindViewById<ImageView>(Resource.Id.movie_details_rating_star_5).SetImageResource(Resource.Drawable.ic_star_crop_half);
			}
		}




		private void buildOverviewView(View layout) {
			if (_movie.Overview != null) {
				TextView overview_view = layout.FindViewById<TextView>(Resource.Id.movie_details_overview);
				overview_view.Text = _movie.Overview;
			}
		}




		private void buildWatchlistButton(View layout) {
			Button watchlist_button = layout.FindViewById<Button>(Resource.Id.movie_details_add_watchlist_button);
			watchlist_button.SetOnClickListener(new WatchlistButtonClickListener(_main_activity, watchlist_button, _movie));
		}




		private void buildRatingButton(View layout) {
			Button rating_button = layout.FindViewById<Button>(Resource.Id.movie_details_add_rating_button);
			rating_button.SetOnClickListener(new RatingButtonClickListener(_main_activity, rating_button, _movie));
		}
	}
}