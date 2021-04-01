using System.Threading.Tasks;
using Android.Animation;
using Android.Content;
using Android.OS;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using Newtonsoft.Json;




namespace Movolira.Pages.ShowDetailsPages {
	public class MovieDetailsFragment : Fragment {
		private readonly int[] RATING_STAR_VIEW_RESOURCE_IDS = {
			Resource.Id.movie_details_rating_star_1,
			Resource.Id.movie_details_rating_star_2,
			Resource.Id.movie_details_rating_star_3,
			Resource.Id.movie_details_rating_star_4,
			Resource.Id.movie_details_rating_star_5,
		};


		private MainActivity _main_activity;
		private Movie _movie;




		public override void OnAttach(Context main_activity) {
			this._main_activity = (MainActivity)main_activity;
			base.OnAttach(main_activity);
		}




		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle saved_instance_state) {
			View layout = inflater.Inflate(Resource.Layout.movie_details_page, container, false);
			// A default "LayoutTransition" object adds some default animations to views entering or leaving containers.
			LayoutTransition layout_transition = new LayoutTransition();
			layout.FindViewById<ViewGroup>(Resource.Id.movie_details_content).LayoutTransition = layout_transition;
			this._movie = JsonConvert.DeserializeObject<Movie>(this.Arguments.GetString("movie"));


			Task.Run(() => this.buildMovieData(layout));
			return layout;
		}




		private async void buildMovieData(View layout) {
			if (!await this._main_activity.MovieProvider.getMovieDetails(this._movie)) {
				this._main_activity.RunOnUiThread(() => {
					this._main_activity.setIsLoading(false);
					this._main_activity.showNetworkError();
				});
				return;
			}


			this._main_activity.RunOnUiThread(() => {
				ShowDetailsViewBuilder.buildBackdropView(this._main_activity, layout,
					this._movie, Resource.Id.movie_details_backdrop);
				ShowDetailsViewBuilder.buildTitleView(this._main_activity, layout, this._movie, Resource.Id.movie_details_title);
				ShowDetailsViewBuilder.buildGenresView(layout, this._movie, Resource.Id.movie_details_genres);
				ShowDetailsViewBuilder.buildCertificationView(layout, this._movie, Resource.Id.movie_details_certification);
				ShowDetailsViewBuilder.buildRatingView(layout, this._movie, Resource.Id.movie_details_rating,
					Resource.Id.movie_details_vote_count, this.RATING_STAR_VIEW_RESOURCE_IDS);
				ShowDetailsViewBuilder.buildOverviewView(layout, this._movie, Resource.Id.movie_details_overview);
				ShowDetailsViewBuilder.buildWatchlistButton(this._main_activity, layout, this._movie,
					Resource.Id.movie_details_add_watchlist_button);
				ShowDetailsViewBuilder.buildRatingButton(this._main_activity, layout, this._movie,
					Resource.Id.movie_details_add_rating_button);


				this.buildReleaseDateView(layout);
				this.buildEPRuntimeView(layout);


				layout.FindViewById<View>(Resource.Id.movie_details_info).Visibility = ViewStates.Visible;
				this._main_activity.setIsLoading(false);
			});
		}




		private void buildReleaseDateView(View layout) {
			TextView release_date_view = layout.FindViewById<TextView>(Resource.Id.movie_details_release_date);
			string release_date_title = "Released\n";
			release_date_view.Text = release_date_title;


			if (this._movie.ReleaseDate != null) {
				release_date_view.Text += this._movie.ReleaseDate;
			} else {
				release_date_view.Text += "-";
			}


			// Enlarges the number value.
			SpannableStringBuilder release_date_styled_string = new SpannableStringBuilder(release_date_view.Text);
			release_date_styled_string.SetSpan(new RelativeSizeSpan(1.2f), release_date_title.Length,
				release_date_styled_string.Length(), SpanTypes.ExclusiveExclusive);
			release_date_view.TextFormatted = release_date_styled_string;
		}




		private void buildEPRuntimeView(View layout) {
			TextView runtime_view = layout.FindViewById<TextView>(Resource.Id.movie_details_runtime);
			string runtime_title = "Runtime\n";
			runtime_view.Text = runtime_title;


			if (this._movie.Runtime > 0) {
				int runtime_hours = this._movie.Runtime / 60;
				int runtime_minutes = this._movie.Runtime % 60;
				runtime_view.Text += runtime_hours + "h " + runtime_minutes + "min";
			} else {
				runtime_view.Text += "-";
			}


			// Enlarges the number value.
			SpannableStringBuilder runtime_styled_string = new SpannableStringBuilder(runtime_view.Text);
			runtime_styled_string.SetSpan(new RelativeSizeSpan(1.2f), runtime_title.Length,
				runtime_styled_string.Length(), SpanTypes.ExclusiveExclusive);
			runtime_view.TextFormatted = runtime_styled_string;
		}
	}
}