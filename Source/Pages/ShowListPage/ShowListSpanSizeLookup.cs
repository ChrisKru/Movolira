using AndroidX.RecyclerView.Widget;




namespace Movolira.Pages.ShowListPage {
	public class ShowListSpanSizeLookup : GridLayoutManager.SpanSizeLookup {
		private ShowCardViewAdapter _cards_view_adapter;
		private int _span_count;




		public ShowListSpanSizeLookup(int cards_view_span_count, ShowCardViewAdapter adapter) {
			this._span_count = cards_view_span_count;
			this._cards_view_adapter = adapter;
		}




		public override int GetSpanSize(int child_position) {
			// If child is a card view.
			if (child_position == this._cards_view_adapter.ItemCount - 1) {
				return this._span_count;


				// If child is the pager view.
			} else {
				return 1;
			}
		}
	}
}