using System;
using Android.Content;
using Android.Graphics;
using Android.Views;
using AndroidX.RecyclerView.Widget;




namespace Movolira.Pages.ShowListPage {
	public class ShowCardViewDecoration : RecyclerView.ItemDecoration {
		private Context _app_context;




		public ShowCardViewDecoration(Context app_context) {
			this._app_context = app_context;
		}




		// Calculates exact card positions, based on screen DPI and previously calculated column number.
		public override void GetItemOffsets(Rect offset_rect, View view,
			RecyclerView parent_view, RecyclerView.State parent_state) {


			float display_density = this._app_context.Resources.DisplayMetrics.Density;
			int offset = (int)(display_density * 14);
			int child_position = parent_view.GetChildLayoutPosition(view);
			int child_count = parent_view.GetAdapter().ItemCount;


			if (child_position == child_count - 1) {
				return;
			}
			int span_count = ((GridLayoutManager)parent_view.GetLayoutManager()).SpanCount;
			int child_column_pos = child_position % span_count;


			offset_rect.Bottom = offset;
			if (child_position < span_count) {
				offset_rect.Top = offset;
			}


			offset_rect.Left = (int)Math.Round((double)
				(span_count - child_column_pos % span_count) / span_count * offset);
			offset_rect.Right = (int)Math.Round((double)
				(child_column_pos + 1) % span_count / span_count * offset);
			if (offset_rect.Right == 0) {
				offset_rect.Right = offset;
			}
		}
	}
}