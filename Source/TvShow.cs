using Newtonsoft.Json;




namespace Movolira {
	public class TvShow : Show {
		public string AirDate { get; set; }
		public double Rating { get; set; }
		public int Votes { get; set; }
		public string Overview { get; set; }
		public int Runtime { get; set; }
		public string Certification { get; set; }




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




		public TvShow(string id, ShowType type, string title) : base(id, type, title) {
			this.AirDate = "";
			this.Rating = 0;
			this.Votes = 0;
			this.Overview = "";
		}




		[JsonConstructor]
		public TvShow(ShowType Type, string Id, string Title, string[] Genres, string PosterUrl,
			string BackdropUrl, string AirDate, double Rating, int Votes, string Overview)
			: base(Id, Type, Title, Genres, PosterUrl, BackdropUrl) {


			this.AirDate = AirDate;
			this.Rating = Rating;
			this.Votes = Votes;
			this.Overview = Overview;
			this.AreShowMainDetailsFetched = true;
		}
	}
}