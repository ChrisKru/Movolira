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
	public class TvShowDetailsFragment : Fragment {
		private readonly int[] RATING_STAR_VIEW_RESOURCE_IDS = {
			Resource.Id.tv_show_details_rating_star_1,
			Resource.Id.tv_show_details_rating_star_2,
			Resource.Id.tv_show_details_rating_star_3,
			Resource.Id.tv_show_details_rating_star_4,
			Resource.Id.tv_show_details_rating_star_5,
		};


		private MainActivity _main_activity;
		private TvShow _tv_show;




		public override void OnAttach(Context main_activity) {
			this._main_activity = (MainActivity)main_activity;
			base.OnAttach(main_activity);
		}




		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle saved_instance_state) {
			View layout = inflater.Inflate(Resource.Layout.tv_show_details_page, container, false);
			// A default "LayoutTransition" object adds some default animations to views entering or leaving containers.
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
				ShowDetailsViewBuilder.buildBackdropView(this._main_activity,
					layout, this._tv_show, Resource.Id.tv_show_details_backdrop);
				ShowDetailsViewBuilder.buildTitleView(this._main_activity, layout, this._tv_show, Resource.Id.tv_show_details_title);
				ShowDetailsViewBuilder.buildGenresView(layout, this._tv_show, Resource.Id.tv_show_details_genres);
				ShowDetailsViewBuilder.buildCertificationView(layout, this._tv_show, Resource.Id.tv_show_details_certification);
				ShowDetailsViewBuilder.buildRatingView(layout, this._tv_show, Resource.Id.tv_show_details_rating,
					Resource.Id.tv_show_details_vote_count, this.RATING_STAR_VIEW_RESOURCE_IDS);
				ShowDetailsViewBuilder.buildOverviewView(layout, this._tv_show, Resource.Id.tv_show_details_overview);
				ShowDetailsViewBuilder.buildWatchlistButton(this._main_activity, layout, this._tv_show,
					Resource.Id.tv_show_details_add_watchlist_button);
				ShowDetailsViewBuilder.buildRatingButton(this._main_activity, layout, this._tv_show,
					Resource.Id.tv_show_details_add_rating_button);


				this.buildAirDateView(layout);
				this.buildRuntimeView(layout);


				layout.FindViewById<View>(Resource.Id.tv_show_details_info).Visibility = ViewStates.Visible;
				this._main_activity.setIsLoading(false);
			});
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


			// Enlarges the number value.
			SpannableStringBuilder air_date_styled_string = new SpannableStringBuilder(air_date_view.Text);
			air_date_styled_string.SetSpan(new RelativeSizeSpan(1.2f), air_date_title.Length, air_date_styled_string.Length(),
				SpanTypes.ExclusiveExclusive);
			air_date_view.TextFormatted = air_date_styled_string;
		}




		private void buildRuntimeView(View layout) {
			TextView runtime_view = layout.FindViewById<TextView>(Resource.Id.tv_show_details_runtime);
			string runtime_title = "EP Runtime\n";
			runtime_view.Text = runtime_title;


			if (this._tv_show.EpisodeRuntime > 0) {
				runtime_view.Text += this._tv_show.EpisodeRuntime + "min";
			} else {
				runtime_view.Text += "-";
			}


			// Enlarges the number value.
			SpannableStringBuilder runtime_styled_string = new SpannableStringBuilder(runtime_view.Text);
			runtime_styled_string.SetSpan(new RelativeSizeSpan(1.2f), runtime_title.Length, runtime_styled_string.Length(),
				SpanTypes.ExclusiveExclusive);
			runtime_view.TextFormatted = runtime_styled_string;
		}
	}
}