using Newtonsoft.Json;

namespace Movolira {
	public class DetailedMovie {
		public string BackdropItemPath { get; }
		public string BackdropPath { get; }
		public string PosterPath { get; }
		public string Title { get; }
		public string Overview { get; }
		public string Genres { get; }
		public string ReleaseDate { get; }
		public double Rating { get; }
		public string Runtime { get; }

		[JsonConstructor]
		public DetailedMovie(string backdrop_item_path, string backdrop_path, string poster_path, string title, string overview, string genres,
		                     string release_date, double rating, string runtime) {
			BackdropItemPath = backdrop_item_path;
			BackdropPath = backdrop_path;
			PosterPath = poster_path;
			Title = title;
			Overview = overview;
			Genres = genres;
			ReleaseDate = release_date;
			Rating = rating;
			Runtime = runtime;
		}
	}
}