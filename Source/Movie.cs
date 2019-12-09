using Newtonsoft.Json;

namespace Movolira {
	public class Movie : Show {
		
		public string ReleaseDate { get; }
		public double Rating { get; }
		public int Votes { get; }
		public string Overview { get; }
		public int Runtime { get; set; }
		public string Certification { get; set; }




		[JsonConstructor]
		public Movie(ShowType Type, string Id, string Title, string[] Genres, string PosterUrl, string BackdropUrl, string ReleaseDate, double Rating, int Votes, string Overview) 
			: base(Id, Type, Title, Genres, PosterUrl, BackdropUrl) 
		{
			this.ReleaseDate = ReleaseDate;
			this.Rating = Rating;
			this.Votes = Votes;
			this.Overview = Overview;
		}
	}
}