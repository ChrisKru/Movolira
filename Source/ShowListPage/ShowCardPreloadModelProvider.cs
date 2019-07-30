using System.Collections.Generic;
using Bumptech.Glide;
using Bumptech.Glide.Load.Resource.Drawable;
using Java.Lang;
using Java.Util;
using IList = System.Collections.IList;

namespace Movolira {
	public class ShowCardPreloadModelProvider : Object, ListPreloader.IPreloadModelProvider {
		public List<Movie> Shows { get; set; }
		private readonly MainActivity _main_activity;

		public ShowCardPreloadModelProvider(List<Movie> shows, MainActivity main_activity) {
			Shows = shows;
			_main_activity = main_activity;
		}

		public IList GetPreloadItems(int position) {
			if (position >= Shows.Count) {
				return Collections.EmptyList();
			}
			string poster_url = Shows[position].PosterUrl;
			if (poster_url == "") {
				return Collections.EmptyList();
			}
			return Collections.SingletonList(poster_url);
		}

		public RequestBuilder GetPreloadRequestBuilder(Object poster_url) {
			return Glide.With(_main_activity).Load(poster_url).Thumbnail(Glide.With(_main_activity)
				.Load(((string) poster_url).Replace("/fanart/", "/preview/")).Transition(DrawableTransitionOptions.WithCrossFade()));
		}
	}
}