using Newtonsoft.Json;
using Realms;




namespace Movolira {
	public class Show {
		public string Id { get; }
		public ShowType Type { get; }
		public string Title { get; }
		public string[] Genres { get; set; }
		public string PosterUrl { get; set; }
		public string BackdropUrl { get; set; }




		public Show(string id, ShowType type, string title) {
			this.Id = id;
			this.Type = type;
			this.Title = title;
			this.PosterUrl = "";
			this.BackdropUrl = "";
		}




		[JsonConstructor]
		public Show(string Id, ShowType Type, string Title, string[] Genres, string PosterUrl, string BackdropUrl) {
			this.Id = Id;
			this.Type = Type;
			this.Title = Title;
			this.Genres = Genres;
			this.PosterUrl = PosterUrl;
			this.BackdropUrl = BackdropUrl;
		}




		public ShowSerialized serialize() {
			ShowSerialized show = new ShowSerialized();
			show.Id = this.Id;
			show.Type = this.Type.ToString();
			show.Title = this.Title;
			show.Genre = this.Genres[0];


			if (show.Genre == "Science Fiction") {
				show.Genre = "Sci-Fi";
			}
			return show;
		}
	}




	public enum ShowType {
		Movie,
		TvShow
	}




	public class ShowSerialized : RealmObject {
		public string Id { get; set; }
		public string Type { get; set; }
		public string Title { get; set; }
		public string Genre { get; set; }
	}




	public class RatedShowSerialized : RealmObject {
		public string Id { get; set; }
		public string Type { get; set; }
		public string Title { get; set; }
		public int Rating { get; set; }
	}




	public class AlreadyRecommendedShow : RealmObject {
		public string Id { get; set; }
	}
}