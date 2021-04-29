# Movolira
Movolira is a mobile movie and tv-show assistant application for Android devices. The application is made with [Xamarin.Android](https://docs.microsoft.com/en-us/xamarin/android/) and is based on [TheMovieDB API](https://developers.themoviedb.org/3/getting-started/introduction). 


## Features
- View show catalogues.
- Search shows by title, genres, rating and release date.
- Rate and watchlist shows.
- Get show recommendations.


## Building
Prerequisites:
- Visual Studio 2019 Community or above.

Building process:
1. Open `Movolira.sln` in `Visual Studio`
2. Create `ApiKeys.cs` file in `/Source/DataProviders/`:
```
namespace Movolira.DataProviders {
	public static class ApiKeys {
		public const string TMDB_KEY = "";
	}
}
``` 
3. Generate a [TMDB API key](https://developers.themoviedb.org/3/getting-started/introduction) and insert it into the `TMDB_KEY` field.
4. Build and run the solution.


## Roadmap
- Dark mode support
- Split show searching into movies/tv-shows.
- Improved recommendation system with a machine learning solution.
- User accounts, cloud saving.


## License
[MIT](https://choosealicense.com/licenses/mit/)
