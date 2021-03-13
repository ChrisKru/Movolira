namespace Movolira {
	// Implemented by fragments, that require specific 'back button' handling.
	public interface IBackButtonHandler {
		bool handleBackButtonPress();
	}
}