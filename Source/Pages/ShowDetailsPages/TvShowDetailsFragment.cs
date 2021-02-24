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




namespace Movolira.Pages.ShowDetailsPages {
	public class TvShowDetailsFragment : Fragment {
		private MainActivity _main_activity;
		private TvShow _tv_show;




		public override void OnAttach(Context main_activity) {
			this._main_activity = (MainActivity)main_activity;
			base.OnAttach(main_activity);
		}




		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle saved_instance_state) {
			View layout = inflater.Inflate(Resource.Layout.tv_show_details_page, container, false);
			LayoutTransition layout_transition = new LayoutTransition();
			layout.FindViewById<ViewGroup>(Resource.Id.tv_show_details_content).LayoutTransition = layout_transition;
			this._tv_show = JsonConvert.DeserializeObject<TvShow>(this.Arguments.GetString("tv_show"));



			Task.Run(() => this.buildTvShowData(layout));
			return layout;
		}




		private async void buildTvShowData(View layout) {
			if (!(await this._main_activity.TvShowProvider.getTvShowDetails(this._tv_show))) {
				this._main_activity.RunOnUiThread(() => {
					this._main_activity.setIsLoading(false);
					this._main_activity.showNetworkError();
				});
				return;
			}


			this._main_activity.RunOnUiThread(() => {
				this.buildBackdropView(layout);
				this.buildTitleView(layout);
				this.buildGenresView(layout);
				this.buildAirDateView(layout);
				this.buildRuntimeView(layout);
				this.buildCertificationView(layout);
				this.buildRatingView(layout);
				this.buildOverviewView(layout);
				this.buildWatchlistButton(layout);
				this.buildRatingButton(layout);


				layout.FindViewById<View>(Resource.Id.tv_show_details_info).Visibility = ViewStates.Visible;
				this._main_activity.setIsLoading(false);
			});
		}




		private void buildBackdropView(View layout) {
			ImageView backdrop_view = layout.FindViewById<ImageView>(Resource.Id.tv_show_details_backdrop);
			RequestOptions image_load_options = new RequestOptions().CenterCrop().Placeholder(new ColorDrawable(Color.Black))
				.Error(new ColorDrawable(Color.LightGray));
			RequestOptions thumbnail_options = new RequestOptions().CenterCrop();


			if (this._main_activity.Resources.Configuration.Orientation == Orientation.Portrait) {
				Glide.With(this._main_activity).Load(this._tv_show.BackdropUrl)
					.Transition(DrawableTransitionOptions.WithCrossFade()).Apply(image_load_options)
					.Thumbnail(Glide.With(this._main_activity)
					.Load(this._tv_show.BackdropUrl.Replace("/fanart/", "/preview/")).Apply(thumbnail_options)
					.Transition(DrawableTransitionOptions.WithCrossFade())).Into(backdrop_view);
			} else {
				Glide.With(this._main_activity).Load(this._tv_show.PosterUrl)
					.Transition(DrawableTransitionOptions.WithCrossFade()).Apply(image_load_options)
					.Thumbnail(Glide.With(this._main_activity)
					.Load(this._tv_show.PosterUrl.Replace("/fanart/", "/preview/")).Apply(thumbnail_options)
					.Transition(DrawableTransitionOptions.WithCrossFade())).Into(backdrop_view);
			}
		}




		private void buildTitleView(View layout) {
			if (this._tv_show.Title != null) {
				TextView title_view = layout.FindViewById<TextView>(Resource.Id.tv_show_details_title);
				title_view.Text = this._tv_show.Title;
				this._main_activity.setToolbarTitle(this._tv_show.Title);


			} else {
				this._main_activity.setToolbarTitle("Movolira");
			}
		}




		private void buildGenresView(View layout) {
			if (this._tv_show.Genres.Length > 0) {
				TextView genres_view = layout.FindViewById<TextView>(Resource.Id.tv_show_details_genres);
				genres_view.Text = this._tv_show.Genres[0].First().ToString().ToUpper()
					+ this._tv_show.Genres[0].Substring(1);


				if (this._tv_show.Genres.Length > 1) {
					genres_view.Text += "\n" + this._tv_show.Genres[1].First().ToString().ToUpper()
						+ this._tv_show.Genres[1].Substring(1);
				}
			}
		}




		private void buildAirDateView(View layout) {
			TextView air_date_view = layout.FindViewById<TextView>(Resource.Id.tv_show_details_air_date);
			string air_date_title = "Aired\n";
			air_date_view.Text = air_date_title;


			if (this._tv_show.AirDate != null) {
				air_date_view.Text += this._tv_show.AirDate;
			} else {
				air_date_view.Text += "-";
			}


			SpannableStringBuilder air_date_styled_string = new SpannableStringBuilder(air_date_view.Text);
			air_date_styled_string.SetSpan(new RelativeSizeSpan(1.2f), air_date_title.Length, air_date_styled_string.Length(),
				SpanTypes.ExclusiveExclusive);
			air_date_view.TextFormatted = air_date_styled_string;
		}




		private void buildRuntimeView(View layout) {
			TextView runtime_view = layout.FindViewById<TextView>(Resource.Id.tv_show_details_runtime);
			string runtime_title = "EP Runtime\n";
			runtime_view.Text = runtime_title;


			if (this._tv_show.Runtime > 0) {
				runtime_view.Text += this._tv_show.Runtime + "min";
			} else {
				runtime_view.Text += "-";
			}


			SpannableStringBuilder runtime_styled_string = new SpannableStringBuilder(runtime_view.Text);
			runtime_styled_string.SetSpan(new RelativeSizeSpan(1.2f), runtime_title.Length, runtime_styled_string.Length(),
				SpanTypes.ExclusiveExclusive);
			runtime_view.TextFormatted = runtime_styled_string;
		}




		private void buildCertificationView(View layout) {
			TextView certification_view = layout.FindViewById<TextView>(Resource.Id.tv_show_details_certification);
			string certification_title = "Certified\n";
			certification_view.Text = certification_title;


			if (this._tv_show.Certification != null) {
				certification_view.Text += this._tv_show.Certification;
			} else {
				certification_view.Text += "-";
			}


			SpannableStringBuilder certification_styled_string = new SpannableStringBuilder(certification_view.Text);
			certification_styled_string.SetSpan(new RelativeSizeSpan(1.2f), certification_title.Length,
				certification_styled_string.Length(), SpanTypes.ExclusiveExclusive);
			certification_view.TextFormatted = certification_styled_string;
		}




		private void buildRatingView(View layout) {
			TextView rating_view = layout.FindViewById<TextView>(Resource.Id.tv_show_details_rating);
			rating_view.Text = $"{this._tv_show.Rating * 10:F0}%";
			TextView vote_count_view = layout.FindViewById<TextView>(Resource.Id.tv_show_details_vote_count);
			vote_count_view.Text = this._tv_show.Votes + " votes";
			layout.FindViewById<ImageView>(Resource.Id.tv_show_details_rating_star_1)
				.SetImageResource(Resource.Drawable.ic_star_crop_full);
			int rating = (int)Math.Floor(this._tv_show.Rating);


			if (rating >= 3) {
				layout.FindViewById<ImageView>(Resource.Id.tv_show_details_rating_star_2)
					.SetImageResource(Resource.Drawable.ic_star_crop_full);
			} else if (rating >= 2) {
				layout.FindViewById<ImageView>(Resource.Id.tv_show_details_rating_star_2)
					.SetImageResource(Resource.Drawable.ic_star_crop_half);
			}


			if (rating >= 5) {
				layout.FindViewById<ImageView>(Resource.Id.tv_show_details_rating_star_3)
					.SetImageResource(Resource.Drawable.ic_star_crop_full);
			} else if (rating >= 4) {
				layout.FindViewById<ImageView>(Resource.Id.tv_show_details_rating_star_3)
					.SetImageResource(Resource.Drawable.ic_star_crop_half);
			}


			if (rating >= 7) {
				layout.FindViewById<ImageView>(Resource.Id.tv_show_details_rating_star_4)
					.SetImageResource(Resource.Drawable.ic_star_crop_full);
			} else if (rating >= 6) {
				layout.FindViewById<ImageView>(Resource.Id.tv_show_details_rating_star_4)
					.SetImageResource(Resource.Drawable.ic_star_crop_half);
			}


			if (rating >= 9) {
				layout.FindViewById<ImageView>(Resource.Id.tv_show_details_rating_star_5)
					.SetImageResource(Resource.Drawable.ic_star_crop_full);
			} else if (rating >= 8) {
				layout.FindViewById<ImageView>(Resource.Id.tv_show_details_rating_star_5)
					.SetImageResource(Resource.Drawable.ic_star_crop_half);
			}
		}




		private void buildWatchlistButton(View layout) {
			Button watchlist_button = layout.FindViewById<Button>(Resource.Id.tv_show_details_add_watchlist_button);
			watchlist_button.SetOnClickListener(new WatchlistButtonClickListener(this._main_activity,
				watchlist_button, this._tv_show));
		}




		private void buildRatingButton(View layout) {
			Button rating_button = layout.FindViewById<Button>(Resource.Id.tv_show_details_add_rating_button);
			rating_button.SetOnClickListener(new RatingButtonClickListener(this._main_activity,
				rating_button, this._tv_show));
		}




		private void buildOverviewView(View layout) {
			if (this._tv_show.Overview != null) {
				TextView overview_view = layout.FindViewById<TextView>(Resource.Id.tv_show_details_overview);
				overview_view.Text = this._tv_show.Overview;
			}
		}
	}
}