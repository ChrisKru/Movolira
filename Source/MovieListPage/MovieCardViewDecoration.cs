using System;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;

namespace Movolira {
	internal class MovieCardViewDecoration : RecyclerView.ItemDecoration {
		private readonly Context _app_context;

		public MovieCardViewDecoration(Context app_context) {
			_app_context = app_context;
		}

		public override void GetItemOffsets(Rect offset_rect, View view, RecyclerView parent_view, RecyclerView.State parent_state) {
			float display_density = _app_context.Resources.DisplayMetrics.Density;
			int offset = (int) (display_density * 14);
			int child_pos = parent_view.GetChildLayoutPosition(view);
			int span_count = ((GridLayoutManager) parent_view.GetLayoutManager()).SpanCount;
			int child_column_pos = child_pos % span_count;
			offset_rect.Bottom = offset;
			if (child_pos < span_count) {
				offset_rect.Top = offset;
			}
			offset_rect.Left = (int) Math.Round((double) (span_count - child_column_pos % span_count) / span_count * offset);
			offset_rect.Right = (int) Math.Round((double) (child_column_pos + 1) % span_count / span_count * offset);
			if (offset_rect.Right == 0) {
				offset_rect.Right = offset;
			}
		}
	}
}