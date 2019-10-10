using Newtonsoft.Json;

namespace Movolira {
	public class Movie : Show {
		public string TraktID { get; }
		public string TMDB_ID { get; }
		public string ReleaseDate { get; }
		public int Runtime { get; }
		public double Rating { get; }
		public int Votes { get; }
		public string Certification { get; }
		public string Overview { get; }
		public string BackdropUrl { get; set; }


		[JsonConstructor]
		public Movie(ShowType Type, string TraktID, string TMDB_ID, string Title, string[] Genres, string ReleaseDate, int Runtime, double Rating,
		             int Votes, string Certification, string Overview) : base(Type, Title, Genres) {
			this.TraktID = TraktID;
			this.TMDB_ID = TMDB_ID;
			this.ReleaseDate = ReleaseDate;
			this.Runtime = Runtime;
			this.Rating = Rating;
			this.Votes = Votes;
			this.Certification = Certification;
			this.Overview = Overview;
		}
	}
}