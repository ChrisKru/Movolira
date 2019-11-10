using Newtonsoft.Json;

namespace Movolira {
	public class TvShow : Show {
		public string Id { get; }
		public string AirDate { get; }
		public double Rating { get; }
		public int Votes { get; }
		public string Overview { get; }
		public string BackdropUrl { get; set; }
		public int Runtime { get; set; }
		public string Certification { get; set; }




		[JsonConstructor]
		public TvShow(ShowType Type, string Id, string Title, string[] Genres, string AirDate, double Rating, int Votes, string Overview) 
			: base(Type, Title, Genres) 
		{
			this.Id = Id;
			this.AirDate = AirDate;
			this.Rating = Rating;
			this.Votes = Votes;
			this.Overview = Overview;
		}
	}
}