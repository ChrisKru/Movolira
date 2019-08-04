using Newtonsoft.Json;

namespace Movolira {
	public class TvShow : Show {
		public string TraktID { get; }
		public string TVDB_ID { get; }
		public string AirDate { get; }
		public int Runtime { get; }
		public double Rating { get; }
		public int Votes { get; }
		public string Certification { get; }
		public string Overview { get; }
		public string BackdropUrl { get; set; }

		[JsonConstructor]
		public TvShow(ShowType Type, string TraktID, string TVDB_ID, string Title, string[] Genres, string AirDate, int Runtime, double Rating,
		              int Votes, string Certification, string Overview) : base(Type, Title, Genres) {
			this.TraktID = TraktID;
			this.TVDB_ID = TVDB_ID;
			this.AirDate = AirDate;
			this.Runtime = Runtime;
			this.Rating = Rating;
			this.Votes = Votes;
			this.Certification = Certification;
			this.Overview = Overview;
		}
	}
}