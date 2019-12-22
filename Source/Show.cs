using Newtonsoft.Json;
using Realms;

namespace Movolira {
	public class Show {
		public string Id { get; }
		public ShowType Type { get; }
		public string Title { get; }
		public string[] Genres { get; }
		public string PosterUrl { get; set; }
		public string BackdropUrl { get; set; }




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
			show.Id = Id;
			show.Type = Type.ToString();
			show.Title = Title;
			show.Genre = Genres[0];


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
}