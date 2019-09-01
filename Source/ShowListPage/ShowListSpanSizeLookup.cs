﻿using Android.Support.V7.Widget;

namespace Movolira {
	public class ShowListSpanSizeLookup : GridLayoutManager.SpanSizeLookup {
		private readonly ShowCardViewAdapter _cards_view_adapter;
		private readonly int _span_count;

		public ShowListSpanSizeLookup(int cards_view_span_count, ShowCardViewAdapter adapter) {
			_span_count = cards_view_span_count;
			_cards_view_adapter = adapter;
		}

		public override int GetSpanSize(int child_position) {
			if (child_position == _cards_view_adapter.ItemCount - 1) {
				return _span_count;
			}
			return 1;
		}
	}
}