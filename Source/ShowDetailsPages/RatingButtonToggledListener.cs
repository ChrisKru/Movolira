using Android.Widget;
using Java.Lang;

namespace Movolira {
	internal class RatingButtonToggledListener : Object, CompoundButton.IOnCheckedChangeListener {
		public void OnCheckedChanged(CompoundButton button_view, bool is_checked) {
			if (is_checked) {
				((ToggleButton) button_view).TextOn = "You rated 98%";
			}
		}
	}
}