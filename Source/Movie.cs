using Newtonsoft.Json;




namespace Movolira {
	public class Movie : Show {
		public string ReleaseDate { get; set; }
		public double Rating { get; set; }
		public int Votes { get; set; }
		public string Overview { get; set; }
		public int Runtime { get; set; }
		public string Certification { get; set; }




		public static Movie deserialize(ShowSerialized show) {
			ShowType type;
			if (show.Type == ShowType.Movie.ToString()) {
				type = ShowType.Movie;
			} else {
				type = ShowType.TvShow;
			}


			return new Movie(show.Id, type, show.Title);
		}




		public static Movie deserialize(RatedShowSerialized show) {
			ShowType type;
			if (show.Type == ShowType.Movie.ToString()) {
				type = ShowType.Movie;
			} else {
				type = ShowType.TvShow;
			}


			return new Movie(show.Id, type, show.Title);
		}




		public Movie(string id, ShowType type, string title) : base(id, type, title) {
			this.ReleaseDate = "";
			this.Rating = 0;
			this.Votes = 0;
			this.Overview = "";
		}




		[JsonConstructor]
		public Movie(ShowType Type, string Id, string Title, string[] Genres, string PosterUrl, 
			string BackdropUrl, string ReleaseDate, double Rating, int Votes, string Overview) 
			: base(Id, Type, Title, Genres, PosterUrl, BackdropUrl) 
		{
			this.ReleaseDate = ReleaseDate;
			this.Rating = Rating;
			this.Votes = Votes;
			this.Overview = Overview;
		}
	}
}