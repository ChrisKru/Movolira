using Newtonsoft.Json;

namespace Movolira {
	public class Movie : Show {
		public string Id { get; }
		public string ReleaseDate { get; }
		public double Rating { get; }
		public int Votes { get; }
		public string Overview { get; }
		public string BackdropUrl { get; set; }
		public int Runtime { get; set; }
		public string Certification { get; set; }


		[JsonConstructor]
		public Movie(ShowType Type, string Id, string Title, string[] Genres, string ReleaseDate, double Rating,
		             int Votes, string Overview) : base(Type, Title, Genres) {
			this.Id = Id;
			this.ReleaseDate = ReleaseDate;
			this.Rating = Rating;
			this.Votes = Votes;
			this.Overview = Overview;
		}
	}
}