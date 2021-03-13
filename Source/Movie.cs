using Newtonsoft.Json;




namespace Movolira {
	public class Movie : Show {
		public string ReleaseDate { get; set; }
		// Runtime is an extra value, which requires separate TMDB API query.
		public int Runtime { get; set; }




		public static Movie deserialize(SerializedShow show) {
			ShowType type;
			if (show.Type == ShowType.Movie.ToString()) {
				type = ShowType.Movie;
			} else {
				type = ShowType.TvShow;
			}


			return new Movie(show.Id, type, show.Title);
		}




		public static Movie deserialize(SerializedRatedShow show) {
			ShowType type;
			if (show.Type == ShowType.Movie.ToString()) {
				type = ShowType.Movie;
			} else {
				type = ShowType.TvShow;
			}


			return new Movie(show.Id, type, show.Title);
		}




		public Movie(string id, ShowType type, string title) : base(id, type, title) { }




		[JsonConstructor]
		public Movie(ShowType Type, string Id, string Title, string[] Genres, string PosterUrl,
			string BackdropUrl, double Rating, int Votes, string Overview, string ReleaseDate)
			: base(Id, Type, Title, Genres, PosterUrl, BackdropUrl, Rating, Votes, Overview) {


			this.ReleaseDate = ReleaseDate;
		}
	}
}