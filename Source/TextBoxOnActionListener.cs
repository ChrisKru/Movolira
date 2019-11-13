
using Android.Content;
using Android.InputMethodServices;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;

namespace Movolira {
	internal class TextBoxOnActionListener : Object, TextView.IOnEditorActionListener {
		private readonly EditText _text_box;




		public TextBoxOnActionListener(EditText text_box) {
			_text_box = text_box;
		}




		public bool OnEditorAction(TextView view, ImeAction action_id, KeyEvent key_event) {
			if (action_id == ImeAction.Done) { 
				_text_box.ClearFocus();
				((ViewGroup) _text_box.Parent).RequestFocus();
				InputMethodManager input_manager = (InputMethodManager) view.Context.GetSystemService(Context.InputMethodService);
				input_manager.HideSoftInputFromWindow(view.WindowToken, 0);
			}
			return false;
		}
	}
}