using Newtonsoft.Json;




namespace Movolira {
	public class TvShow : Show {
		public string AirDate { get; set; }
		// EpisodeRuntime is an extra value, which requires separate TMDB API query.
		public int EpisodeRuntime { get; set; }




		public static TvShow deserialize(SerializedShow show) {
			ShowType type;
			if (show.Type == ShowType.Movie.ToString()) {
				type = ShowType.Movie;
			} else {
				type = ShowType.TvShow;
			}


			return new TvShow(show.Id, type, show.Title);
		}




		public static TvShow deserialize(SerializedRatedShow show) {
			ShowType type;
			if (show.Type == ShowType.Movie.ToString()) {
				type = ShowType.Movie;
			} else {
				type = ShowType.TvShow;
			}


			return new TvShow(show.Id, type, show.Title);
		}




		public TvShow(string id, ShowType type, string title) : base(id, type, title) { }




		[JsonConstructor]
		public TvShow(ShowType Type, string Id, string Title, string[] Genres, string PosterUrl,
			string BackdropUrl, double Rating, int Votes, string Overview, string AirDate)
			: base(Id, Type, Title, Genres, PosterUrl, BackdropUrl, Rating, Votes, Overview) {


			this.AirDate = AirDate;
		}
	}
}