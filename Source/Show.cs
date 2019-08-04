using Newtonsoft.Json;

namespace Movolira {
	public class Show {
		public ShowType Type { get; }
		public string Title { get; }
		public string[] Genres { get; }
		public string PosterUrl { get; set; }

		[JsonConstructor]
		public Show(ShowType Type, string Title, string[] Genres) {
			this.Type = Type;
			this.Title = Title;
			this.Genres = Genres;
		}
	}

	public enum ShowType {
		Movie,
		TvShow
	}
}