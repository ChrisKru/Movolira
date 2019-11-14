using Newtonsoft.Json;

namespace Movolira {
	public class Show {
		public ShowType Type { get; }
		public string Title { get; }
		public string[] Genres { get; }
		public string PosterUrl { get; set; }
		public string BackdropUrl { get; set; }




		[JsonConstructor]
		public Show(ShowType Type, string Title, string[] Genres, string PosterUrl, string BackdropUrl) {
			this.Type = Type;
			this.Title = Title;
			this.Genres = Genres;
			this.PosterUrl = PosterUrl;
			this.BackdropUrl = BackdropUrl;
		}
	}


	public enum ShowType {
		Movie,
		TvShow
	}
}