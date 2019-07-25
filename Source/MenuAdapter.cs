using System.Collections.Generic;
using Android.Database;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Movolira {
	internal class MenuAdapter : Object, IExpandableListAdapter {
		public int GroupCount => _group_items.Count;
		public bool HasStableIds => false;
		public bool IsEmpty => _group_items.Count == 0;

		private readonly Dictionary<int, List<string>> _child_items = new Dictionary<int, List<string>> {
			[0] = new List<string> {"Popular", "Trending"},
			[1] = new List<string> {"Popular", "Trending"}
		};

		private readonly List<string> _group_items = new List<string> {
			"Movies", "TV Shows"
		};

		private readonly MainActivity _main_activity;

		public MenuAdapter(MainActivity activity) {
			_main_activity = activity;
		}

		public bool AreAllItemsEnabled() {
			return true;
		}

		public Object GetChild(int group_pos, int child_pos) {
			return null;
		}

		public long GetChildId(int group_pos, int child_pos) {
			return child_pos;
		}

		public int GetChildrenCount(int group_pos) {
			return _child_items[group_pos].Count;
		}

		public View GetChildView(int group_pos, int child_pos, bool is_last_child, View convert_view, ViewGroup parent) {
			if (convert_view == null) {
				convert_view = _main_activity.LayoutInflater.Inflate(Resource.Layout.menu_child, null);
			}
			TextView text_view = convert_view.FindViewById<TextView>(Resource.Id.menu_child_text_view);
			text_view.Text = _child_items[group_pos][child_pos];
			return convert_view;
		}

		public long GetCombinedChildId(long group_id, long child_id) {
			return group_id * 100 + child_id;
		}

		public long GetCombinedGroupId(long group_id) {
			return 10000 + group_id;
		}

		public Object GetGroup(int group_pos) {
			return null;
		}

		public long GetGroupId(int group_pos) {
			return group_pos;
		}

		public View GetGroupView(int group_pos, bool is_expanded, View convert_view, ViewGroup parent) {
			if (convert_view == null) {
				convert_view = _main_activity.LayoutInflater.Inflate(Resource.Layout.menu_group, null);
			}
			TextView text_view = convert_view.FindViewById<TextView>(Resource.Id.menu_group_text_view);
			text_view.Text = _group_items[group_pos];
			return convert_view;
		}

		public bool IsChildSelectable(int group_pos, int child_pos) {
			return true;
		}

		public void OnGroupCollapsed(int group_pos) { }

		public void OnGroupExpanded(int group_pos) { }

		public void RegisterDataSetObserver(DataSetObserver observer) { }

		public void UnregisterDataSetObserver(DataSetObserver observer) { }
	}
}