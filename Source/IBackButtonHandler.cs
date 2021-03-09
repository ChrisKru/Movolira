namespace Movolira{
	// Implemented by fragments, that require specific 'back button' handling.
	internal interface IBackButtonHandler {
		bool handleBackButtonPress();
	}
}