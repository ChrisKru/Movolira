using Android.Views;
using Android.Widget;
using Java.Lang;
using Math = System.Math;

/*
namespace Movolira {
	public class OverviewViewSpanModifier : Object, ViewTreeObserver.IOnGlobalLayoutListener {
		private readonly View _layout;

		public OverviewViewSpanModifier(View layout) {
			_layout = layout;
		}

		public void OnGlobalLayout() {
			_layout.ViewTreeObserver.RemoveOnGlobalLayoutListener(this);
			ImageView poster_view = _layout.FindViewById<ImageView>(Resource.Id.movie_details_poster);
			TextView overview_view = _layout.FindViewById<TextView>(Resource.Id.movie_details_overview);
			TextView overview_unconstrained = _layout.FindViewById<TextView>(Resource.Id.movie_details_overview_unconstrained);
			if (poster_view.Bottom < overview_view.Bottom) {
				int constrained_text_height = poster_view.Bottom - overview_view.Top;
				int constrained_text_line_count;
				if (constrained_text_height < 0) {
					constrained_text_line_count = 0;
				} else {
					constrained_text_line_count = (int) Math.Ceiling((double) constrained_text_height / overview_view.LineHeight);
					if (constrained_text_line_count > overview_view.LineCount) {
						constrained_text_line_count = overview_view.LineCount;
					}
				}
				int constrained_text_end = overview_view.Layout.GetLineEnd(constrained_text_line_count - 1);
				overview_unconstrained.Text = overview_view.Text.Substring(constrained_text_end);
				overview_view.Text = overview_view.Text.Substring(0, constrained_text_end);
				if (constrained_text_line_count == 0) {
					overview_view.Visibility = ViewStates.Gone;
				}
			}
		}
	}
}*/