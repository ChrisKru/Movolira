using Newtonsoft.Json;
using Realms;




namespace Movolira {
	public class Show {
		// "Main Details" refers to the fields that are bundled together with page listing requests of TMDB API.
		// The "AreShowMainDetailsFetched" field is used to omit reinitializing those fields.
		public bool AreShowMainDetailsFetched { get; set; } = false;
		public string Id { get; }
		public ShowType Type { get; }
		public string Title { get; }
		public string[] Genres { get; set; }
		public string PosterUrl { get; set; }
		public string BackdropUrl { get; set; }
		public double Rating { get; set; }
		public int Votes { get; set; }
		public string Overview { get; set; }
		// Certification is an extra value, which requires separate TMDB API query.
		public string Certification { get; set; }




		public Show(string id, ShowType type, string title) {
			this.Id = id;
			this.Type = type;
			this.Title = title;
		}




		[JsonConstructor]
		public Show(string Id, ShowType Type, string Title, string[] Genres, string PosterUrl,
			string BackdropUrl, double Rating, int Votes, string Overview) {


			this.AreShowMainDetailsFetched = true;
			this.Id = Id;
			this.Type = Type;
			this.Title = Title;
			this.Genres = Genres;
			this.PosterUrl = PosterUrl;
			this.BackdropUrl = BackdropUrl;
			this.Rating = Rating;
			this.Votes = Votes;
			this.Overview = Overview;
		}




		public SerializedShow serialize() {
			SerializedShow show = new SerializedShow();
			show.Id = this.Id;
			show.Type = this.Type.ToString();
			show.Title = this.Title;
			// Saves only a single, most accurate genre, which will be displayed in the listings.
			show.Genre = this.Genres[0];


			return show;
		}
	}




	public enum ShowType {
		Movie,
		TvShow
	}




	// Each class is used to categorize different database entries in the Realm database API
	public class SerializedShow : RealmObject {
		public string Id { get; set; }
		public string Type { get; set; }
		public string Title { get; set; }
		public string Genre { get; set; }
	}




	public class SerializedRatedShow : RealmObject {
		public string Id { get; set; }
		public string Type { get; set; }
		public string Title { get; set; }
		public int Rating { get; set; }
	}




	public class AlreadyRecommendedShow : RealmObject {
		public string Id { get; set; }
	}
}