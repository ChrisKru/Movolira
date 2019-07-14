using Android.Content;
using Android.Content.Res;
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
			float density = _app_context.Resources.DisplayMetrics.Density;
			int offset = (int) (density * 7);
			int child_pos = parent_view.GetChildLayoutPosition(view);
			int child_count = parent_state.ItemCount;
			if (_app_context.Resources.Configuration.Orientation == Orientation.Landscape) {
				if (child_pos < 4) {
					offset_rect.Top = 2 * offset;
					offset_rect.Bottom = offset;
				} else if (child_count - 4 <= child_pos) {
					offset_rect.Top = offset;
					offset_rect.Bottom = 2 * offset;
				} else {
					offset_rect.Top = offset;
					offset_rect.Bottom = offset;
				}

				if (child_pos % 4 == 0) {
					offset_rect.Left = 2 * offset;
					offset_rect.Right = offset;
				} else if ((child_pos + 1) % 4 == 0) {
					offset_rect.Left = offset;
					offset_rect.Right = 2 * offset;
				} else {
					offset_rect.Left = offset;
					offset_rect.Right = offset;
				}
			} else {
				if (child_pos < 2) {
					offset_rect.Top = 2 * offset;
					offset_rect.Bottom = offset;
				} else if (child_count - 2 <= child_pos) {
					offset_rect.Top = offset;
					offset_rect.Bottom = 2 * offset;
				} else {
					offset_rect.Top = offset;
					offset_rect.Bottom = offset;
				}

				if (child_pos % 2 == 0) {
					offset_rect.Left = 2 * offset;
					offset_rect.Right = offset;
				} else {
					offset_rect.Left = offset;
					offset_rect.Right = 2 * offset;
				}
			}
		}
	}
}