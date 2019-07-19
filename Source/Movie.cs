using Newtonsoft.Json;

namespace Movolira {
	public class Movie {
		public string TraktID { get; }
		public string TMDB_ID { get; }
		public string Title { get; }
		public string[] Genres { get; }
		public string ReleaseDate { get; }
		public int Runtime { get; }
		public double Rating { get; }
		public int Votes { get; }
		public string Certification { get; }
		public string Overview { get; }
		public string PosterUrl { get; }
		public string BackdropUrl { get; }

		[JsonConstructor]
		public Movie(string TraktID, string TMDB_ID, string Title, string[] Genres, string ReleaseDate, int Runtime, double Rating, int Votes,
		             string Certification, string Overview, string PosterUrl, string BackdropUrl) {
			this.TraktID = TraktID;
			this.TMDB_ID = TMDB_ID;
			this.Title = Title;
			this.Genres = Genres;
			this.ReleaseDate = ReleaseDate;
			this.Runtime = Runtime;
			this.Rating = Rating;
			this.Votes = Votes;
			this.Certification = Certification;
			this.Overview = Overview;
			this.PosterUrl = PosterUrl;
			this.BackdropUrl = BackdropUrl;
		}
	}
}