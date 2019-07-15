using Newtonsoft.Json;

namespace Movolira {
	public class DetailedMovie {
		public string BackdropSmallUrl { get; }
		public string BackdropUrl { get; }
		public string PosterUrl { get; }
		public string Title { get; }
		public string Overview { get; }
		public string Genres { get; }
		public string ReleaseDate { get; }
		public double Rating { get; }
		public string Runtime { get; }

		[JsonConstructor]
		public DetailedMovie(string backdrop_small_url, string backdrop_url, string poster_url, string title, string overview, string genres,
		                     string release_date, double rating, string runtime) {
			BackdropSmallUrl = backdrop_small_url;
			BackdropUrl = backdrop_url;
			PosterUrl = poster_url;
			Title = title;
			Overview = overview;
			Genres = genres;
			ReleaseDate = release_date;
			Rating = rating;
			Runtime = runtime;
		}
	}
}